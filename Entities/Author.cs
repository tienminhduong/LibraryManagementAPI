namespace LibraryManagementAPI.Entities;

public class Author
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int YearOfBirth { get; set; }
    public string? BriefDescription { get; set; }
    public ICollection<Book> Books { get; set; } = [];
}