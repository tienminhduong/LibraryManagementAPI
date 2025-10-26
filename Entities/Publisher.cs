namespace LibraryManagementAPI.Entities;

public class Publisher
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Address { get; set; }
    public ICollection<Book> Books { get; set; } = [];
}