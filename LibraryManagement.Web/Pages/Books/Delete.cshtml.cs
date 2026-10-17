using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Books;

public class DeleteModel : PageModel
{
    private readonly LibraryContext _context;

    public DeleteModel(LibraryContext context)
    {
        _context = context;
    }

    public Book Book { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
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

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .Include(b => b.Loans)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return RedirectToPage("./Index");
        }

        // Loan history must be kept (DeleteBehavior.Restrict backs this up)
        if (book.Loans.Any())
        {
            Book = book;
            ModelState.AddModelError(string.Empty,
                "This book has loan history and can't be deleted.");
            return Page();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        StatusMessage = $"Book '{book.Title}' was deleted.";
        return RedirectToPage("./Index");
    }
}