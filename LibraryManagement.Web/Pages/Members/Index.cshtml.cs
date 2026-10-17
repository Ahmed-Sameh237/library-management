using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Members;

public class IndexModel : PageModel
{
    private readonly LibraryContext _context;

    public IndexModel(LibraryContext context)
    {
        _context = context;
    }

    public IList<Member> Members { get; set; } = new List<Member>();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Members = await _context.Members
            .Include(m => m.Loans)      // needed for ActiveLoanCount
            .OrderBy(m => m.FullName)
            .AsNoTracking()
            .ToListAsync();
    }
}