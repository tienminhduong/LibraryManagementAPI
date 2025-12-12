namespace LibraryManagementAPI.Models.Book;

public class CreateBookImportDetailsDto
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
}