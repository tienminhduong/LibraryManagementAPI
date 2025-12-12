using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public enum BorrowRequestStatus
    {
        Pending,          // Request created, waiting for staff confirmation
        Borrowed,         // Book copy is currently borrowed and not overdue
        Overdue,          // Book copy is borrowed but past due date
        Returned,         // Book copy has been returned on time
        OverdueReturned,  // Book copy has been returned but was overdue
        Rejected,         // Request was rejected by staff
        Cancelled         // Request was cancelled by member
    }

    /// <summary>
    /// Represents a single book copy borrow request.
    /// Each book copy borrowed creates a separate BorrowRequest.
    /// If a member borrows 3 books, 3 BorrowRequest records are created.
    /// </summary>
    public class BorrowRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Guid MemberId { get; set; }
        public Guid? StaffId { get; set; }
        public Guid? ProcessedByAccountId { get; set; } // New: account id of actor (staff or admin)
        
        // Single book and copy per request
        public Guid BookId { get; set; }
        public Guid? BookCopyId { get; set; } // Assigned when confirmed by staff
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; } // When staff confirms
        public DateTime? BorrowDate { get; set; } // When actually borrowed (confirmed)
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; } // When book was returned
        
        public BorrowRequestStatus Status { get; set; } = BorrowRequestStatus.Pending;
        public string? QrCode { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("MemberId")]
        public MemberInfo? Member { get; set; }
        
        [ForeignKey("StaffId")]
        public StaffInfo? Staff { get; set; }

        // Note: ProcessedByAccountId references Accounts table (no FK configured)

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
        
        [ForeignKey("BookCopyId")]
        public BookCopy? BookCopy { get; set; }
    }
}
