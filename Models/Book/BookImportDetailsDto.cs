namespace LibraryManagementAPI.Models.Book;

public class BookImportDetailsDto
{
    public BookDto Book { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}