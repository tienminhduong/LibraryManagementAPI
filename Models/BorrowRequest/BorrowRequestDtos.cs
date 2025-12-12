using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Models.BorrowRequest
{
    /// <summary>
    /// DTO for member to create multiple borrow requests (one per book)
    /// </summary>
    public class CreateBorrowRequestDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one book must be selected")]
        public List<Guid> BookIds { get; set; } = new List<Guid>();
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for admin to create a single borrow request with specific book copy
    /// </summary>
    public class AdminCreateBorrowRequestDto
    {
        [Required]
        public Guid MemberId { get; set; }
        
        [Required]
        public Guid BookCopyId { get; set; } // Single copy
        
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for member search results
    /// </summary>
    public class MemberSearchDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }

    /// <summary>
    /// Single borrow request DTO (one book copy)
    /// </summary>
    public class BorrowRequestDto
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public Guid? StaffId { get; set; }
        public string? StaffName { get; set; }
        
        // Single book copy info
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookISBN { get; set; }
        public string? BookImageUrl { get; set; }
        public Guid? BookCopyId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? BorrowDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        
        public string Status { get; set; } = string.Empty;
        public string? QrCode { get; set; }
        public string? Notes { get; set; }
        public bool IsOverdue { get; set; }
    }

    /// <summary>
    /// Response after creating borrow request(s)
    /// </summary>
    public class BorrowRequestResponseDto
    {
        public List<Guid> RequestIds { get; set; } = new List<Guid>();
        public List<string> QrCodes { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
    }

    /// <summary>
    /// DTO for staff to confirm a single borrow request with specific book copy
    /// </summary>
    public class ConfirmBorrowRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }
        
        [Required]
        public Guid BookCopyId { get; set; } // Staff assigns specific copy
    }

    /// <summary>
    /// DTO for returning a book copy
    /// </summary>
    public class ReturnBookDto
    {
        [Required]
        public Guid BookCopyId { get; set; }
    }

    /// <summary>
    /// Result of book return operation
    /// </summary>
    public class ReturnBookResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? MemberName { get; set; }
        public string? BookTitle { get; set; }
        public Guid? BorrowRequestId { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public bool WasOverdue { get; set; }
    }

    /// <summary>
    /// Grouped borrow requests for display (multiple copies from same checkout session)
    /// </summary>
    public class GroupedBorrowRequestDto
    {
        public DateTime CreatedAt { get; set; }
        public Guid MemberId { get; set; }
        public string? MemberName { get; set; }
        public List<BorrowRequestDto> Requests { get; set; } = new List<BorrowRequestDto>();
        public int TotalBooks { get; set; }
        public string OverallStatus { get; set; } = string.Empty; // All Pending, All Borrowed, Mixed, etc.
    }
}
