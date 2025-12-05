using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementAPI.Entities
{
    /// <summary>
    /// Cart entity for database storage
    /// Represents a member's shopping cart for books they want to borrow
    /// </summary>
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Account ID of the member (not Info ID)
        /// </summary>
        public Guid AccountId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

    /// <summary>
    /// Cart item entity - represents a book in the cart
    /// </summary>
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid CartId { get; set; }
        public Guid BookId { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }

        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}
