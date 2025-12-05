namespace LibraryManagementAPI.Models.Cart;

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
    public Guid AccountId { get; set; }  // Changed from MemberId to AccountId
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
