namespace LibraryManagementAPI.Models.Supplier
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
    }
}
