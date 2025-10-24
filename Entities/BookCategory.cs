namespace LibraryManagementAPI.Entities;


public class BookCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<Book> Books { get; set; } = [];
}