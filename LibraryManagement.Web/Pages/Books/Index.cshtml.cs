using LibraryManagement.Web.Data;
using LibraryManagement.Web.Helpers;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Books;

public class IndexModel : PageModel
{
    private const int PageSize = 5;   // small on purpose so you can see paging with 6 books

    private readonly LibraryContext _context;

    public IndexModel(LibraryContext context)
    {
        _context = context;
    }

    // GET filters, read from the query string
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool OnlyAvailable { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public PaginatedList<BookListItem> Books { get; set; } = default!;
    public SelectList CategoryOptions { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        CategoryOptions = new SelectList(
            await _context.Categories.OrderBy(c => c.Name).AsNoTracking().ToListAsync(),
            nameof(Category.Id), nameof(Category.Name));

        // Nothing hits the database while we build the query up
        IQueryable<Book> query = _context.Books;

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var term = Search.Trim();
            query = query.Where(b =>
                b.Title.Contains(term) ||
                b.Isbn.Contains(term) ||
                b.Author!.Name.Contains(term));
        }

        if (CategoryId.HasValue)
        {
            query = query.Where(b => b.Categories.Any(c => c.Id == CategoryId.Value));
        }

        if (OnlyAvailable)
        {
            query = query.Where(b => b.TotalCopies > b.Loans.Count(l => l.ReturnedOn == null));
        }

        // Project only the columns the list needs (no Include, no loading whole Loan rows)
        var items = query
            .OrderBy(b => b.Title)
            .Select(b => new BookListItem
            {
                Id = b.Id,
                Title = b.Title,
                AuthorName = b.Author!.Name,
                TotalCopies = b.TotalCopies,
                ActiveLoans = b.Loans.Count(l => l.ReturnedOn == null),
                Categories = b.Categories.Select(c => c.Name).ToList()
            });

        // The query runs here, inside CreateAsync (count, then one page)
        Books = await PaginatedList<BookListItem>.CreateAsync(items, PageNumber, PageSize);
    }
}