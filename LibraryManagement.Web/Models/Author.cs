using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models;

public class Author
{
	public int Id { get; set; }

	[Required, StringLength(100)]
	public string Name { get; set; } = string.Empty;

	[StringLength(1000)]
	public string? Biography { get; set; }

	// Navigation property: one author has many books
	public ICollection<Book> Books { get; set; } = new List<Book>();
}