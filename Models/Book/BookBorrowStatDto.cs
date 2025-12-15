


namespace LibraryManagementAPI.Models.Book
{
    public class BookBorrowStatDto
    {
        public LibraryManagementAPI.Entities.Book book { get; set; }
        public int BorrowCount { get; set; }
    }
}
