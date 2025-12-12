namespace LibraryManagementAPI.Models.Supplier;

public class SupplierDto
{
    public Guid id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
}