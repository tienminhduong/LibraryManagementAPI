namespace LibraryManagementAPI.Models.BookCopy;

public class BookCopyDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string? BookTitle { get; set; }
    public string? BookISBN { get; set; }
    public string Status { get; set; } = string.Empty;
    public string QrCode { get; set; } = string.Empty;
}
