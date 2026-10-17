using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Authors;

public class IndexModel : PageModel
{
    private readonly LibraryContext _context;

    public IndexModel(LibraryContext context)
    {
        _context = context;
    }

    public IList<Author> Authors { get; set; } = new List<Author>();

    // Survives exactly one redirect (used for the PRG success message)
    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Authors = await _context.Authors
            .Include(a => a.Books)
            .OrderBy(a => a.Name)
            .AsNoTracking()
            .ToListAsync();
    }
}