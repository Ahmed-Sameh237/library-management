using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Members;

public class EditModel : PageModel
{
    private readonly LibraryContext _context;

    public EditModel(LibraryContext context)
    {
        _context = context;
    }

    public Member Member { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var member = await _context.Members
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
        var memberToUpdate = await _context.Members.FindAsync(id);
        if (memberToUpdate == null)
        {
            return NotFound();
        }

        var isValid = await TryUpdateModelAsync(
            memberToUpdate, "Member",
            m => m.FullName, m => m.Email, m => m.Phone);

        if (isValid)
        {
            memberToUpdate.Email = memberToUpdate.Email.Trim();

            if (await _context.Members.AnyAsync(m => m.Email == memberToUpdate.Email && m.Id != id))
            {
                ModelState.AddModelError("Member.Email", "Another member already uses this email.");
                isValid = false;
            }
        }

        if (!isValid)
        {
            Member = memberToUpdate;
            return Page();
        }

        await _context.SaveChangesAsync();

        StatusMessage = $"Member '{memberToUpdate.FullName}' was updated.";
        return RedirectToPage("./Index");
    }
}