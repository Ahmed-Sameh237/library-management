using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
namespace LibraryManagement.Web.Pages.Books;

public abstract class BookFormPageModel : PageModel
{
    protected readonly LibraryContext _context;

    protected BookFormPageModel(LibraryContext context)
    {
        _context = context;
    }

    // Deliberately NOT [BindProperty]: we bind it ourselves with TryUpdateModelAsync
    public Book Book { get; set; } = new();

    // Checkbox values posted from the form
    [BindProperty]
    public List<int> SelectedCategoryIds { get; set; } = new();

    public SelectList AuthorOptions { get; set; } = default!;
    public List<Category> AllCategories { get; set; } = new();

    protected async Task PopulateListsAsync()
    {
        var authors = await _context.Authors
            .OrderBy(a => a.Name)
            .AsNoTracking()
            .ToListAsync();

        AuthorOptions = new SelectList(authors, nameof(Author.Id), nameof(Author.Name));

        AllCategories = await _context.Categories
            .OrderBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync();
    }

    protected Task<bool> IsIsbnTakenAsync(string isbn, int? excludeBookId = null)
    {
        return _context.Books.AnyAsync(b => b.Isbn == isbn && b.Id != excludeBookId);
    }
}