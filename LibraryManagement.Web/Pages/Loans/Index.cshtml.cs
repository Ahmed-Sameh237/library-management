using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Pages.Loans;

public class IndexModel : PageModel
{
	private readonly LibraryContext _context;
	private readonly ILoanService _loanService;

	public IndexModel(LibraryContext context, ILoanService loanService)
	{
		_context = context;
		_loanService = loanService;
	}

	public IList<Loan> Loans { get; set; } = new List<Loan>();

	// "active" (default), "overdue" or "all". Read from the query string.
	[BindProperty(SupportsGet = true)]
	public string Filter { get; set; } = "active";

	[TempData]
	public string? StatusMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	public async Task OnGetAsync()
	{
		IQueryable<Loan> query = _context.Loans
			.Include(l => l.Book)
			.Include(l => l.Member);

		// IsActive / IsOverdue are [NotMapped], so EF can't translate them to SQL.
		// We repeat the logic as expressions EF understands.
		query = Filter switch
		{
			"overdue" => query.Where(l => l.ReturnedOn == null && l.DueDate < DateTime.Today),
			"all" => query,
			_ => query.Where(l => l.ReturnedOn == null)
		};

		Loans = await query
			.OrderByDescending(l => l.BorrowedOn)
			.AsNoTracking()
			.ToListAsync();
	}

	// Handler name "Return" is matched by asp-page-handler="Return" in the form
	public async Task<IActionResult> OnPostReturnAsync(int id)
	{
		var result = await _loanService.ReturnAsync(id);

		if (result.Success)
		{
			StatusMessage = $"'{result.Loan!.Book!.Title}' was returned by {result.Loan.Member!.FullName}.";
		}
		else
		{
			ErrorMessage = result.Error;
		}

		return RedirectToPage(new { filter = Filter });
	}
}