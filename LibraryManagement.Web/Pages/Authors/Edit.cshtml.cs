using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Authors;

public class EditModel : PageModel
{
    private readonly LibraryContext _context;

    public EditModel(LibraryContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Author Author { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author == null)
        {
            return NotFound();
        }

        Author = author;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var authorToUpdate = await _context.Authors.FindAsync(id);
        if (authorToUpdate == null)
        {
            return NotFound();
        }

        // Only these two properties can be changed by the form
        if (await TryUpdateModelAsync(
                authorToUpdate,
                "Author",
                a => a.Name, a => a.Biography))
        {
            await _context.SaveChangesAsync();
            StatusMessage = $"Author '{authorToUpdate.Name}' was updated.";
            return RedirectToPage("./Index");
        }

        Author = authorToUpdate;
        return Page();
    }
}