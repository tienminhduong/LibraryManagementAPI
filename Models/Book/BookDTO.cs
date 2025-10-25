using LibraryManagementAPI.Models.BookCategory;

namespace LibraryManagementAPI.Models.Book;

public class BookDto
{
    public Guid Id { get; set; }
    public required string ISBN { get; set; }
    public required string Title { get; set; }
    public string? ImgUrl { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public int PublicationYear { get; set; }
    public string? Description { get; set; }
    public IEnumerable<BookCategoryDto> BookCategories { get; set; } = [];
}
