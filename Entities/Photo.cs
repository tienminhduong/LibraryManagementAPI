using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    public class Photo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public Guid BookId { get; set; }

        // Navigation property
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}
