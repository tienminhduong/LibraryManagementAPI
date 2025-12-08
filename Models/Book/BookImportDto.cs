namespace LibraryManagementAPI.Models.Book;

public class BookImportDto
{
    public Guid SupplierId { get; set; }
    public Guid StaffId { get; set; }
    public string Notes { get; set; } = "";
    public IEnumerable<BookImportDetailsDto> Details { get; set; } = [];
}