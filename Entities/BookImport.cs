namespace LibraryManagementAPI.Entities
{
    public class bookImport
    {
        public Guid id { get; set; }
        public Guid supplierId { get; set; }
        public Guid staffId { get; set; }
        public DateTime importDate { get; set; } = DateTime.UtcNow;
        public decimal totalAmount { get; set; }
        public string? note { get; set; }

        // Navigation properties
        public Supplier? supplier { get; set; }
        public StaffInfo? staff { get; set; }
        public ICollection<BookImportDetail>? bookImportDetails { get; set; }
    }
}
