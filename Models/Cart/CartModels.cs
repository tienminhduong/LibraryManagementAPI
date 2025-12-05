namespace LibraryManagementAPI.Models.Cart;

/// <summary>
/// Represents a member's shopping cart for books they want to borrow
/// </summary>
public class Cart
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; } // MemberInfo ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}

/// <summary>
/// Represents a single book in the cart
/// </summary>
public class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid BookId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// DTO for adding a book to cart
/// </summary>
public class AddToCartDto
{
    public Guid BookId { get; set; }
}

/// <summary>
/// DTO for cart response
/// </summary>
public class CartDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public int TotalItems => Items.Count;
}

/// <summary>
/// DTO for cart item response
/// </summary>
public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string? BookTitle { get; set; }
    public string? BookISBN { get; set; }
    public string? BookImageUrl { get; set; }
    public DateTime AddedAt { get; set; }
    public bool IsAvailable { get; set; }
}
