namespace LibraryManagementAPI.Models.BorrowRequest
{
    public class CreateBorrowRequestDto
    {
        public List<Guid> BookIds { get; set; } = new List<Guid>();
        public string? Notes { get; set; }
    }

    public class BorrowRequestDto
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public Guid? StaffId { get; set; }
        public string? StaffName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? QrCode { get; set; }
        public string? Notes { get; set; }
        public List<BorrowRequestItemDto> Items { get; set; } = new List<BorrowRequestItemDto>();
    }

    public class BorrowRequestItemDto
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookISBN { get; set; }
        public Guid? BookCopyId { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class BorrowRequestResponseDto
    {
        public Guid RequestId { get; set; }
        public string QrCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ConfirmBorrowRequestDto
    {
        public Guid RequestId { get; set; }
        // StaffId removed - will be retrieved from JWT
        public List<BookCopyAssignmentDto> BookCopyAssignments { get; set; } = new List<BookCopyAssignmentDto>();
    }

    public class BookCopyAssignmentDto
    {
        public Guid BookId { get; set; }
        public Guid BookCopyId { get; set; }
    }

    public class ReturnBookDto
    {
        public Guid BookCopyId { get; set; }
        // StaffId removed - will be retrieved from JWT
    }
}
