using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Books;

public class IndexModel : PageModel
{
    private readonly LibraryContext _context;

    public IndexModel(LibraryContext context)
    {
        _context = context;
    }

    public IList<Book> Books { get; set; } = new List<Book>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Books = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .Include(b => b.Loans)      // needed for AvailableCopies
            .OrderBy(b => b.Title)
            .AsNoTracking()
            .ToListAsync();
    }
}