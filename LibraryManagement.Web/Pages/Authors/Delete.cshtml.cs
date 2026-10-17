using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Authors;

public class DeleteModel : PageModel
{
    private readonly LibraryContext _context;

    public DeleteModel(LibraryContext context)
    {
        _context = context;
    }

    public Author Author { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return NotFound();
        }

        Author = author;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author == null)
        {
            return RedirectToPage("./Index");
        }

        // Business rule backed by DeleteBehavior.Restrict in the database
        if (author.Books.Any())
        {
            Author = author;
            ModelState.AddModelError(string.Empty,
                "This author still has books. Delete or reassign them first.");
            return Page();
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        StatusMessage = $"Author '{author.Name}' was deleted.";
        return RedirectToPage("./Index");
    }
}