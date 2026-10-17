using LibraryManagement.Web.Data;
using LibraryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Services;

public class LoanService : ILoanService
{
    public const int MaxActiveLoans = 3;
    public const int LoanDays = 14;

    private readonly LibraryContext _context;

    public LoanService(LibraryContext context)
    {
        _context = context;
    }

    public async Task<LoanResult> BorrowAsync(int bookId, int memberId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null)
            return LoanResult.Fail("The selected book doesn't exist.");

        var member = await _context.Members.FindAsync(memberId);
        if (member == null)
            return LoanResult.Fail("The selected member doesn't exist.");

        // Rule 1: a member can have at most 3 active loans
        var memberActiveLoans = await _context.Loans
            .CountAsync(l => l.MemberId == memberId && l.ReturnedOn == null);

        if (memberActiveLoans >= MaxActiveLoans)
            return LoanResult.Fail(
                $"{member.FullName} already has {MaxActiveLoans} active loans, which is the maximum.");

        // Rule 2: the same member can't hold two copies of the same book
        var alreadyHasBook = await _context.Loans
            .AnyAsync(l => l.MemberId == memberId && l.BookId == bookId && l.ReturnedOn == null);

        if (alreadyHasBook)
            return LoanResult.Fail($"{member.FullName} has already borrowed '{book.Title}'.");

        // Rule 3: there must be a free copy
        var copiesOut = await _context.Loans
            .CountAsync(l => l.BookId == bookId && l.ReturnedOn == null);

        if (copiesOut >= book.TotalCopies)
            return LoanResult.Fail($"No copies of '{book.Title}' are available right now.");

        // All rules passed: create the loan (due in 14 days)
        var loan = new Loan
        {
            Book = book,
            Member = member,
            BorrowedOn = DateTime.Today,
            DueDate = DateTime.Today.AddDays(LoanDays)
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return LoanResult.Ok(loan);
    }

    public async Task<LoanResult> ReturnAsync(int loanId)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
            return LoanResult.Fail("This loan doesn't exist.");

        if (loan.ReturnedOn != null)
            return LoanResult.Fail("This book was already returned.");

        loan.ReturnedOn = DateTime.Today;
        await _context.SaveChangesAsync();

        return LoanResult.Ok(loan);
    }
}