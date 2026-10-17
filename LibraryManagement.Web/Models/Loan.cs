using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Web.Models;

public class Loan
{
    public int Id { get; set; }

    public int BookId { get; set; }
    public Book? Book { get; set; }

    public int MemberId { get; set; }
    public Member? Member { get; set; }

    public DateTime BorrowedOn { get; set; } = DateTime.Today;
    public DateTime DueDate { get; set; }

    // null = still borrowed
    public DateTime? ReturnedOn { get; set; }

    // Computed in C#, not stored in the database
    [NotMapped]
    public bool IsActive => ReturnedOn == null;

    [NotMapped]
    public bool IsOverdue => ReturnedOn == null && DueDate < DateTime.Today;
}