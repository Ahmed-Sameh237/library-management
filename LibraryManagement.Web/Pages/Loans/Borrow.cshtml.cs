using System.ComponentModel.DataAnnotations;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Loans;

public class BorrowModel : PageModel
{
    private readonly LibraryContext _context;
    private readonly ILoanService _loanService;

    public BorrowModel(LibraryContext context, ILoanService loanService)
    {
        _context = context;
        _loanService = loanService;
    }

    // SupportsGet lets /Loans/Borrow?bookId=3 preselect a book
    [BindProperty(SupportsGet = true)]
    [Required(ErrorMessage = "Please select a book.")]
    [Display(Name = "Book")]
    public int? BookId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Please select a member.")]
    [Display(Name = "Member")]
    public int? MemberId { get; set; }

    public List<SelectListItem> BookOptions { get; set; } = new();
    public List<SelectListItem> MemberOptions { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await PopulateListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await PopulateListsAsync();
            return Page();
        }

        var result = await _loanService.BorrowAsync(BookId!.Value, MemberId!.Value);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error!);
            await PopulateListsAsync();
            return Page();
        }

        var loan = result.Loan!;
        StatusMessage = $"'{loan.Book!.Title}' was borrowed by {loan.Member!.FullName}. " +
                        $"Due on {loan.DueDate:yyyy-MM-dd}.";
        return RedirectToPage("./Index");
    }

    private async Task PopulateListsAsync()
    {
        var books = await _context.Books
            .Include(b => b.Loans)
            .OrderBy(b => b.Title)
            .AsNoTracking()
            .ToListAsync();

        BookOptions = books.Select(b => new SelectListItem
        {
            Value = b.Id.ToString(),
            Text = $"{b.Title} ({b.AvailableCopies} available)",
            Disabled = b.AvailableCopies <= 0
        }).ToList();

        var members = await _context.Members
            .Include(m => m.Loans)
            .OrderBy(m => m.FullName)
            .AsNoTracking()
            .ToListAsync();

        MemberOptions = members.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.FullName} ({m.ActiveLoanCount}/{LoanService.MaxActiveLoans} loans)",
            Disabled = m.ActiveLoanCount >= LoanService.MaxActiveLoans
        }).ToList();
    }
}