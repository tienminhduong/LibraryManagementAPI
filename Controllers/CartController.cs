using LibraryManagementAPI.Authorization;
using LibraryManagementAPI.Extensions;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementAPI.Controllers;

/*
 * ============================================================================
 * COMPLETE BORROW FLOW WITH CART SYSTEM
 * ============================================================================
 * 
 * MEMBER FLOW (Recommended - Using Cart):
 * ----------------------------------------
 * 1. Browse books: GET /api/books
 * 2. Add books to cart: POST /api/cart/add { "bookId": "..." }
 * 3. View cart: GET /api/cart
 * 4. Remove unwanted books: DELETE /api/cart/remove/{cartItemId}
 * 5. Checkout cart: POST /api/cart/checkout { "notes": "..." }
 * 6. Receive QR code in response
 * 7. View request: GET /api/borrow-requests/my-requests
 * 8. Show QR code to librarian at library
 * 
 * MEMBER FLOW (Alternative - Direct Request):
 * --------------------------------------------
 * 1. Browse books: GET /api/books
 * 2. Create request directly: POST /api/borrow-requests 
 *    { "bookIds": ["...", "..."], "notes": "..." }
 * 3. Receive QR code in response
 * 4. Show QR code to librarian
 * 
 * LIBRARIAN FLOW (Processing Request):
 * -------------------------------------
 * 1. Member shows QR code
 * 2. Scan QR code: GET /api/borrow-requests/qr/{qrCode}
 * 3. System displays list of requested books
 * 4. For each book:
 *    - Find available physical copy on shelf
 *    - Scan BookCopy QR code (COPY-{guid})
 *    - Record BookCopyId
 * 5. Confirm request: POST /api/borrow-requests/confirm
 *    {
 *      "requestId": "...",
 *      "bookCopyAssignments": [
 *        { "bookId": "...", "bookCopyId": "..." },
 *        { "bookId": "...", "bookCopyId": "..." }
 *      ]
 *    }
 * 6. Books are now borrowed, member takes them home
 * 
 * RETURN FLOW:
 * ------------
 * 1. Member brings books back
 * 2. Librarian scans BookCopy QR code
 * 3. Return book: POST /api/borrow-requests/return { "bookCopyId": "..." }
 * 4. Book status updated to Available
 * 
 * QR CODE TYPES:
 * --------------
 * - Borrow Request QR: BORROW-{requestId}
 * - BookCopy QR: COPY-{bookCopyId}
 * 
 * CART STORAGE:
 * -------------
 * - Currently using in-memory dictionary (static)
 * - In production, should use:
 *   - Database table (CartEntity with CartItems)
 *   - OR Redis/Distributed Cache for better scalability
 * 
 * ============================================================================
 */

/// <summary>
/// Cart Management Controller
/// Handles book cart operations before creating borrow requests
/// </summary>
[ApiController]
[Route("api/cart")]
[Authorize(Policy = Policies.MemberOnly)] // Only members can manage their cart
public class CartController(ICartService cartService) : ControllerBase
{
    /// <summary>
    /// Get the authenticated member's cart with all books
    /// Flow: Member views their cart to see selected books before requesting
    /// </summary>
    /// <returns>Cart with list of books</returns>
    [HttpGet]
    public async Task<IActionResult> GetMyCart()
    {
        try
        {
            // Extract member's account ID from JWT token
            var accountId = User.GetUserId();
            
            // Retrieve cart from storage (in-memory or database)
            var cart = await cartService.GetCartByAccountIdAsync(accountId);
            
            return Ok(cart);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Add a book to cart
    /// Flow: 
    /// 1. Member browses books
    /// 2. Member clicks "Add to Cart" on a book
    /// 3. Book is added to their cart for later borrow request
    /// </summary>
    /// <param name="dto">Contains BookId to add</param>
    /// <returns>Updated cart with new book added</returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        try
        {
            // Get authenticated member's account ID from JWT
            var accountId = User.GetUserId();
            
            // Add book to cart
            // - Validates book exists
            // - Checks if book has available copies
            // - Prevents duplicate books in cart
            var cart = await cartService.AddToCartAsync(accountId, dto.BookId);
            
            return Ok(new 
            { 
                message = "Book added to cart successfully.",
                cart 
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove a specific book from cart
    /// Flow: Member removes unwanted books before creating borrow request
    /// </summary>
    /// <param name="cartItemId">ID of the cart item to remove</param>
    /// <returns>Success status</returns>
    [HttpDelete("remove/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
    {
        try
        {
            // Get authenticated member's account ID from JWT
            var accountId = User.GetUserId();
            
            // Remove specific item from cart
            var result = await cartService.RemoveFromCartAsync(accountId, cartItemId);
            
            if (result)
                return Ok(new { message = "Book removed from cart successfully." });
            else
                return NotFound(new { message = "Cart item not found." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Clear all books from cart
    /// Flow: Member wants to start fresh and clear entire cart
    /// </summary>
    /// <returns>Success status</returns>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            // Get authenticated member's account ID from JWT
            var accountId = User.GetUserId();
            
            // Remove all items from cart
            var result = await cartService.ClearCartAsync(accountId);
            
            if (result)
                return Ok(new { message = "Cart cleared successfully." });
            else
                return NotFound(new { message = "Cart not found." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create borrow request from cart items
    /// Flow:
    /// 1. Member adds books to cart (using /api/cart/add)
    /// 2. Member reviews cart (using /api/cart)
    /// 3. Member creates borrow request from cart (this endpoint)
    /// 4. Cart is automatically cleared after successful request
    /// 5. Member receives QR code to show librarian
    /// </summary>
    /// <param name="notes">Optional notes for the borrow request</param>
    /// <returns>Success status</returns>
    [HttpPost("checkout")]
    public async Task<IActionResult> CheckoutCart([FromBody] CheckoutCartDto dto)
    {
        try
        {
            // Get authenticated member's account ID from JWT
            var accountId = User.GetUserId();
            
            // Create borrow request from all books in cart
            // - Validates all books still have available copies
            // - Creates BorrowRequest with QR code
            // - Clears cart after successful creation
            var result = await cartService.CreateBorrowRequestFromCartAsync(accountId, dto.Notes);
            
            if (result)
                return Ok(new 
                { 
                    message = "Borrow request created successfully from cart. Cart has been cleared. Check 'My Requests' for QR code." 
                });
            else
                return BadRequest(new { message = "Failed to create borrow request from cart." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

/// <summary>
/// DTO for checkout request
/// </summary>
public class CheckoutCartDto
{
    public string? Notes { get; set; }
}
