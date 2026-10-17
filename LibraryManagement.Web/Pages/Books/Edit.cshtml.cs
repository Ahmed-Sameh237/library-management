using LibraryManagement.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Books;

public class EditModel : BookFormPageModel
{
    public EditModel(LibraryContext context) : base(context)
    {
    }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Categories)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        Book = book;
        SelectedCategoryIds = book.Categories.Select(c => c.Id).ToList();
        await PopulateListsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var bookToUpdate = await _context.Books
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bookToUpdate == null)
        {
            return NotFound();
        }

        var isValid = await TryUpdateModelAsync(
            bookToUpdate, "Book",
            b => b.Title, b => b.Isbn, b => b.PublishedYear,
            b => b.TotalCopies, b => b.AuthorId);

        if (isValid && await IsIsbnTakenAsync(bookToUpdate.Isbn, id))
        {
            ModelState.AddModelError("Book.Isbn", "Another book already uses this ISBN.");
            isValid = false;
        }

        // Business rule: can't set total copies below the number currently borrowed
        if (isValid)
        {
            var activeLoans = await _context.Loans
                .CountAsync(l => l.BookId == id && l.ReturnedOn == null);

            if (bookToUpdate.TotalCopies < activeLoans)
            {
                ModelState.AddModelError("Book.TotalCopies",
                    $"{activeLoans} copies are currently borrowed, so total copies can't be lower than that.");
                isValid = false;
            }
        }

        if (!isValid)
        {
            Book = bookToUpdate;
            await PopulateListsAsync();
            return Page();
        }

        // Sync the many-to-many links: replace the old set with the selected set
        var selected = await _context.Categories
            .Where(c => SelectedCategoryIds.Contains(c.Id))
            .ToListAsync();

        bookToUpdate.Categories.Clear();
        foreach (var category in selected)
        {
            bookToUpdate.Categories.Add(category);
        }

        await _context.SaveChangesAsync();

        StatusMessage = $"Book '{bookToUpdate.Title}' was updated.";
        return RedirectToPage("./Index");
    }
}