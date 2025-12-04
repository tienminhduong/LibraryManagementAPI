using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public enum Status
    {
        Available,
        Borrowed,
        Reserved,
        Lost,
        Damaged,
        Cleared
    }
    public class BookCopy
    {
        public Guid id { get; set; }
        public Guid bookId { get; set; }
        public Guid bookImportDetailId { get; set; }
        public Status status { get; set; }

        // Navigation properties
        [ForeignKey("bookId")]
        public Book? book { get; set; }
        
        // Computed property - not stored in database
        [NotMapped]
        public string QrCode => $"COPY-{id}";
    }
}
