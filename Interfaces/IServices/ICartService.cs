using LibraryManagementAPI.Models.Cart;

namespace LibraryManagementAPI.Interfaces.IServices;

/// <summary>
/// Service for managing member's book cart before creating borrow request
/// </summary>
public interface ICartService
{
    /// <summary>
    /// Get member's cart by account ID
    /// </summary>
    Task<CartDto?> GetCartByAccountIdAsync(Guid accountId);
    
    /// <summary>
    /// Add a book to member's cart
    /// </summary>
    Task<CartDto> AddToCartAsync(Guid accountId, Guid bookId);
    
    /// <summary>
    /// Remove a book from cart
    /// </summary>
    Task<bool> RemoveFromCartAsync(Guid accountId, Guid cartItemId);
    
    /// <summary>
    /// Clear all items from cart
    /// </summary>
    Task<bool> ClearCartAsync(Guid accountId);
    
    /// <summary>
    /// Create borrow request from cart items
    /// </summary>
    Task<bool> CreateBorrowRequestFromCartAsync(Guid accountId, string? notes);
}
