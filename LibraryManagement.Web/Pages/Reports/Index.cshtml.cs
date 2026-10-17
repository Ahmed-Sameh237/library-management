using LibraryManagement.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Reports;

public record BookStat(string Title, int Count);
public record MemberStat(string Name, int Total, int Active);
public record CategoryStat(string Name, int Count);
public record OverdueItem(string Book, string Member, DateTime DueDate, int DaysOverdue);

public class IndexModel : PageModel
{
    private readonly LibraryContext _context;

    public IndexModel(LibraryContext context)
    {
        _context = context;
    }

    public int TotalTitles { get; set; }
    public int TotalCopies { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueCount { get; set; }

    public List<BookStat> MostBorrowed { get; set; } = new();
    public List<MemberStat> LoansPerMember { get; set; } = new();
    public List<CategoryStat> LoansByCategory { get; set; } = new();
    public List<OverdueItem> Overdue { get; set; } = new();

    public async Task OnGetAsync()
    {
        // Capture once: EF turns this into a SQL parameter, and every query agrees on "today"
        var today = DateTime.Today;

        // --- Summary numbers ---
        TotalTitles = await _context.Books.CountAsync();
        TotalCopies = await _context.Books.SumAsync(b => b.TotalCopies);
        ActiveLoans = await _context.Loans.CountAsync(l => l.ReturnedOn == null);
        OverdueCount = await _context.Loans
            .CountAsync(l => l.ReturnedOn == null && l.DueDate < today);

        // --- Most borrowed books: Join + GroupBy (query syntax) ---
        MostBorrowed = await (
            from l in _context.Loans
            join b in _context.Books on l.BookId equals b.Id
            group b by new { b.Id, b.Title } into g
            orderby g.Count() descending, g.Key.Title
            select new BookStat(g.Key.Title, g.Count()))
            .Take(5)
            .ToListAsync();

        // --- Loans per member: aggregates over a navigation collection ---
        LoansPerMember = await _context.Members
            .OrderByDescending(m => m.Loans.Count)
            .ThenBy(m => m.FullName)
            .Select(m => new MemberStat(
                m.FullName,
                m.Loans.Count,
                m.Loans.Count(l => l.ReturnedOn == null)))
            .ToListAsync();

        // --- Loans by category: SelectMany across Book -> Loans ---
        LoansByCategory = await _context.Categories
            .OrderByDescending(c => c.Books.SelectMany(b => b.Loans).Count())
            .ThenBy(c => c.Name)
            .Select(c => new CategoryStat(
                c.Name,
                c.Books.SelectMany(b => b.Loans).Count()))
            .ToListAsync();

        // --- Overdue loans: SQL does the filtering, C# does the date math ---
        var overdueRows = await _context.Loans
            .Where(l => l.ReturnedOn == null && l.DueDate < today)
            .OrderBy(l => l.DueDate)
            .Select(l => new { Book = l.Book!.Title, Member = l.Member!.FullName, l.DueDate })
            .ToListAsync();

        Overdue = overdueRows
            .Select(o => new OverdueItem(o.Book, o.Member, o.DueDate, (today - o.DueDate).Days))
            .ToList();
    }
}