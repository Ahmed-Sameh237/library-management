using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Books;

public class DetailsModel : PageModel
{
    private readonly LibraryContext _context;

    public DetailsModel(LibraryContext context)
    {
        _context = context;
    }

    public Book Book { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .Include(b => b.Loans)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        Book = book;
        return Page();
    }
}