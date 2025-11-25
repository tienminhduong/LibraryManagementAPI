using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public class BookImportDetail
    {
        public Guid id { get; set; }
        public Guid bookImportId { get; set; }
        public Guid bookId { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }

        // Navigation properties
        [ForeignKey("bookImportId")]
        public bookImport? bookImport { get; set; }
        [ForeignKey("bookId")]
        public Book? book { get; set; }
    }
}
