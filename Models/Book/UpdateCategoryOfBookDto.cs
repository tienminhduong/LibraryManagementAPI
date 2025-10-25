namespace LibraryManagementAPI.Models.Book;

public class UpdateCategoryOfBookDto
{
    public required IEnumerable<Guid> CategoryIds { get; set; }
}