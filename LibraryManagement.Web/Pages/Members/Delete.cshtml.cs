using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Members;

public class DeleteModel : PageModel
{
    private readonly LibraryContext _context;

    public DeleteModel(LibraryContext context)
    {
        _context = context;
    }

    public Member Member { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await _context.Members
            .Include(m => m.Loans)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return NotFound();
        }

        Member = member;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var member = await _context.Members
            .Include(m => m.Loans)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member == null)
        {
            return RedirectToPage("./Index");
        }

        // Loan history must be kept (DeleteBehavior.Restrict backs this up)
        if (member.Loans.Any())
        {
            Member = member;
            ModelState.AddModelError(string.Empty,
                "This member has loan history and can't be deleted.");
            return Page();
        }

        _context.Members.Remove(member);
        await _context.SaveChangesAsync();

        StatusMessage = $"Member '{member.FullName}' was deleted.";
        return RedirectToPage("./Index");
    }
}