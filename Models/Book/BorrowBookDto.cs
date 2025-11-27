namespace LibraryManagementAPI.Models.Book
{
    public class BorrowBookDto
    {
        public Guid MemberId { get; set; }
        public Guid BookId { get; set; }
        public Guid StaffId { get; set; }
    }
}
