using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.Web.Pages.Authors;

public class CreateModel : PageModel
{
    private readonly LibraryContext _context;

    public CreateModel(LibraryContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Author Author { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();   // redisplay the form with validation errors
        }

        _context.Authors.Add(Author);
        await _context.SaveChangesAsync();

        StatusMessage = $"Author '{Author.Name}' was created.";
        return RedirectToPage("./Index");   // PRG: redirect after a successful POST
    }
}