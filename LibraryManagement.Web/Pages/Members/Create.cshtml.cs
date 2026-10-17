using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Members;

public class CreateModel : PageModel
{
    private readonly LibraryContext _context;

    public CreateModel(LibraryContext context)
    {
        _context = context;
    }

    // Not [BindProperty]: bound manually with TryUpdateModelAsync
    public Member Member { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var newMember = new Member();   // JoinedOn defaults to today

        var isValid = await TryUpdateModelAsync(
            newMember, "Member",
            m => m.FullName, m => m.Email, m => m.Phone);

        if (isValid)
        {
            newMember.Email = newMember.Email.Trim();

            if (await _context.Members.AnyAsync(m => m.Email == newMember.Email))
            {
                ModelState.AddModelError("Member.Email", "A member with this email already exists.");
                isValid = false;
            }
        }

        if (!isValid)
        {
            Member = newMember;
            return Page();
        }

        _context.Members.Add(newMember);
        await _context.SaveChangesAsync();

        StatusMessage = $"Member '{newMember.FullName}' was created.";
        return RedirectToPage("./Index");
    }
}