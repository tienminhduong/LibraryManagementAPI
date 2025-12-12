using LibraryManagementAPI.Models.Info;
using LibraryManagementAPI.Models.Supplier;

namespace LibraryManagementAPI.Models.Book;

public class DetailBookImportDto
{
    public Guid Id { get; set; }
    public SupplierDto Supplier { get; set; }
    public StaffInfoDto Staff { get; set; }
    public DateTime ImportDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public IEnumerable<BookImportDetailsDto>? BookImportDetails { get; set; }
}