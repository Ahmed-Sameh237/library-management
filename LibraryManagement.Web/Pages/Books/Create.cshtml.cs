using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Books;

public class CreateModel : BookFormPageModel
{
    public CreateModel(LibraryContext context) : base(context)
    {
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await PopulateListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var newBook = new Book();

        // Bind only the fields we allow (prevents overposting)
        var isValid = await TryUpdateModelAsync(
            newBook, "Book",
            b => b.Title, b => b.Isbn, b => b.PublishedYear,
            b => b.TotalCopies, b => b.AuthorId);

        if (isValid && await IsIsbnTakenAsync(newBook.Isbn))
        {
            ModelState.AddModelError("Book.Isbn", "A book with this ISBN already exists.");
            isValid = false;
        }

        if (!isValid)
        {
            Book = newBook;
            await PopulateListsAsync();
            return Page();
        }

        // Assigning tracked Category entities creates the join-table rows
        newBook.Categories = await _context.Categories
            .Where(c => SelectedCategoryIds.Contains(c.Id))
            .ToListAsync();

        _context.Books.Add(newBook);
        await _context.SaveChangesAsync();

        StatusMessage = $"Book '{newBook.Title}' was created.";
        return RedirectToPage("./Index");
    }
}