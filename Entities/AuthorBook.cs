using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public class AuthorBook
    {
        public Guid BookId { get; set; }
        public Guid AuthorId { get; set; }

        // Navigation properties
        [ForeignKey("BookId")]
        public  Book? Book { get; set; }
        [ForeignKey("AuthorId")]
        public  Author? Author { get; set; }
    }
}
