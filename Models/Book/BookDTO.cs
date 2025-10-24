using LibraryManagementAPI.Models.BookCategory;

namespace LibraryManagementAPI.Models.Book;

public class BookDTO
{
    public Guid Id { get; set; }
    public required string ISBN { get; set; }
    public required string Title { get; set; }
    public string? ImgUrl { get; set; }
    public BookCategoryDto? Category { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int PublicationYear { get; set; }
    public string? Description { get; set; }
}
