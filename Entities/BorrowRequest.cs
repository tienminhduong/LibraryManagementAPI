using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public enum BorrowRequestStatus
    {
        Pending,
        Confirmed,
        Rejected,
        Cancelled
    }

    public class BorrowRequest
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Guid? StaffId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public BorrowRequestStatus Status { get; set; } = BorrowRequestStatus.Pending;
        public string? QrCode { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("MemberId")]
        public MemberInfo? Member { get; set; }
        
        [ForeignKey("StaffId")]
        public StaffInfo? Staff { get; set; }
        
        public ICollection<BorrowRequestItem> Items { get; set; } = new List<BorrowRequestItem>();
    }
}
