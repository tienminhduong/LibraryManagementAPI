namespace LibraryManagementAPI.Models.Book;

public class BookImportDetailsDto
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
}