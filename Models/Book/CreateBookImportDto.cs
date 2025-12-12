namespace LibraryManagementAPI.Models.Book;

public class CreateBookImportDto
{
    public Guid SupplierId { get; set; }
    public string Notes { get; set; } = "";
    public IEnumerable<CreateBookImportDetailsDto> Details { get; set; } = [];
}