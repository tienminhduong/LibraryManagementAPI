using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public class BorrowRequestItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BorrowRequestId { get; set; }
        public Guid BookId { get; set; }
        public Guid? BookCopyId { get; set; } // Assigned when admin confirms
        public bool IsConfirmed { get; set; } = false;

        // Navigation properties
        [ForeignKey("BorrowRequestId")]
        public BorrowRequest? BorrowRequest { get; set; }
        
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
        
        [ForeignKey("BookCopyId")]
        public BookCopy? BookCopy { get; set; }
    }
}
