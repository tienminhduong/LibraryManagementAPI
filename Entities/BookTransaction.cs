using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public enum StatusTransaction {BORROWED, 
                            RETURNED,
                            OVERDUE};
    public class BookTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public Guid copyId { get; set; }
        public Guid memberId { get; set; }
        public Guid staffId { get; set; }
        public DateTime borrowDate { get; set; } = DateTime.UtcNow;
        public DateTime dueDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public DateTime? returnDate { get; set; } = null;
        public StatusTransaction status { get; set; } = StatusTransaction.BORROWED;
        // Navigation properties
        [ForeignKey("copyId")]
        public BookCopy? book { get; set; }
        [ForeignKey("memberId")]
        public MemberInfo? member { get; set; }
        [ForeignKey("staffId")]
        public StaffInfo? staff { get; set; }
    }
}
