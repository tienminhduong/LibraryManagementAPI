namespace LibraryManagementAPI.Entities;

public class Book
{
    public Guid Id { get; set; }
    public required string ISBN { get; set; }
    public required string Title { get; set; }
    public string? ImgUrl { get; set; }
    public Guid CategoryId { get; set; }
    public BookCategory? Category { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int PublicationYear { get; set; }
    public string? Description { get; set; }
}