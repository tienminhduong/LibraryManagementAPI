using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public class BookCategoryMap
    {
        public Guid BookId { get; set; }
        public Guid CategoryId { get; set; }

        // Navigation properties
        [ForeignKey("BookId")]
        public Book? Book { get; set; } = null!;
        [ForeignKey("CategoryId")]
        public BookCategory? Category { get; set; } = null!;
    }
}
