namespace LibraryManagementAPI.Models.Book;

public class CreateBookDto
{
    public required string ISBN { get; set; }
    public required string Title { get; set; }
    public string? ImgUrl { get; set; }
    public IEnumerable<Guid> CategoryIds { get; set; } = [];
    public IEnumerable<Guid> AuthorIds { get; set; } = [];
    public int PublicationYear { get; set; }
    public string? Description { get; set; }
}
