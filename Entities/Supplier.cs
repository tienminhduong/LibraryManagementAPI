namespace LibraryManagementAPI.Entities
{
    public class Supplier
    {
        public Guid id { get; set; }
        public required string name { get; set; }
        public string? address { get; set; }
        public required string phoneNumber { get; set; }
        public required string email { get; set; }
        public ICollection<BookImport>? bookImports { get; set; }
    }
}
