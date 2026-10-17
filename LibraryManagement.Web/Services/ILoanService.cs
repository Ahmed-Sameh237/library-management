using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Services;

public record LoanResult(bool Success, string? Error = null, Loan? Loan = null)
{
    public static LoanResult Ok(Loan loan) => new(true, null, loan);
    public static LoanResult Fail(string error) => new(false, error);
}

public interface ILoanService
{
    Task<LoanResult> BorrowAsync(int bookId, int memberId);
    Task<LoanResult> ReturnAsync(int loanId);
}