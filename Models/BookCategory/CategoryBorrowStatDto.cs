namespace LibraryManagementAPI.Models.BookCategory
{
    public class CategoryBorrowStatDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BorrowCount { get; set; }
    }
}