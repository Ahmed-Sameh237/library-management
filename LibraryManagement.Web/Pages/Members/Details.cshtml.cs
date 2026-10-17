using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Members;

public class DetailsModel : PageModel
{
    private readonly LibraryContext _context;

    public DetailsModel(LibraryContext context)
    {
        _context = context;
    }

    public Member Member { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await _context.Members
            .Include(m => m.Loans)
                .ThenInclude(l => l.Book)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        Member = member;
        return Page();
    }
}