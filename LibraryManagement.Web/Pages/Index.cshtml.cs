using LibraryManagement.Web.Data;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages;

public record CategoryChip(int Id, string Name, int BookCount);

public class IndexModel : PageModel
{
    private readonly LibraryContext _context;

    public IndexModel(LibraryContext context)
    {
        _context = context;
    }

    public int TitleCount { get; set; }
    public int MemberCount { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueCount { get; set; }

    public List<CategoryChip> Categories { get; set; } = new();
    public List<BookListItem> Latest { get; set; } = new();

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;

        TitleCount = await _context.Books.CountAsync();
        MemberCount = await _context.Members.CountAsync();
        ActiveLoans = await _context.Loans.CountAsync(l => l.ReturnedOn == null);
        OverdueCount = await _context.Loans
            .CountAsync(l => l.ReturnedOn == null && l.DueDate < today);

        Categories = await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryChip(c.Id, c.Name, c.Books.Count))
            .ToListAsync();

        // Newest books first (highest Id), reusing the lightweight model from Step 9
        Latest = await _context.Books
            .OrderByDescending(b => b.Id)
            .Take(4)
            .Select(b => new BookListItem
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author!.Name,
                TotalCopies = b.TotalCopies,
                ActiveLoans = b.Loans.Count(l => l.ReturnedOn == null),
                Categories = b.Categories.Select(c => c.Name).ToList()
            })
            .ToListAsync();
    }
}