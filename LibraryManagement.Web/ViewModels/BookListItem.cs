namespace LibraryManagement.Web.ViewModels;

public class BookListItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public int ActiveLoans { get; set; }
    public List<string> Categories { get; set; } = new();

    public int AvailableCopies => TotalCopies - ActiveLoans;
}