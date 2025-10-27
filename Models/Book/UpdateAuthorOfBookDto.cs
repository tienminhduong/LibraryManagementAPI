namespace LibraryManagementAPI.Models.Book;

public class UpdateAuthorOfBookDto
{
    public required IEnumerable<Guid> AuthorIds { get; set; }
}