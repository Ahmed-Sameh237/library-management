using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    // Many-to-many: a category has many books
    public ICollection<Book> Books { get; set; } = new List<Book>();
}