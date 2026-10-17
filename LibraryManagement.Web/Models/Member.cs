using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryManagement.Web.Models;

public class Member
{
	public int Id { get; set; }

	[Required, StringLength(100)]
	[Display(Name = "Full Name")]
	public string FullName { get; set; } = string.Empty;

	[Required, EmailAddress, StringLength(150)]
	public string Email { get; set; } = string.Empty;

	[Phone, StringLength(20)]
	public string? Phone { get; set; }

	[DataType(DataType.Date)]
	[Display(Name = "Joined On")]
	public DateTime JoinedOn { get; set; } = DateTime.Today;

	public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    // Computed, not stored. Only correct when Loans has been loaded with Include.
    [NotMapped]
    public int ActiveLoanCount => Loans.Count(l => l.ReturnedOn == null);
}