using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Web.Models;

public class Book
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "Enter a valid year.")]
    [Display(Name = "Published Year")]
    public int PublishedYear { get; set; }

    [Range(1, 100)]
    [Display(Name = "Total Copies")]
    public int TotalCopies { get; set; } = 1;

    // Foreign key + navigation (one-to-many with Author)
    [Display(Name = "Author")]
    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    // Many-to-many with Category (EF Core creates the join table for us)
    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    // Computed, not stored. Only correct when Loans has been loaded with Include.
    [NotMapped]
    public int AvailableCopies => TotalCopies - Loans.Count(l => l.ReturnedOn == null);
}