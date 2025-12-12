namespace LibraryManagementAPI.Models.Book;

public class SimpleBookImportDetailsDto
{
    public required string BookTitle { get; set; }
    public required string BookISBN { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}