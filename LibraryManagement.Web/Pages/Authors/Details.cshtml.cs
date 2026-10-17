using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Authors;

public class DetailsModel : PageModel
{
    private readonly LibraryContext _context;

    public DetailsModel(LibraryContext context)
    {
        _context = context;
    }

    public Author Author { get; set; } = default!;

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
}