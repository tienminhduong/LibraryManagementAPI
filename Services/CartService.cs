using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.BorrowRequest;
using LibraryManagementAPI.Models.Cart;

namespace LibraryManagementAPI.Services;

/// <summary>
/// Service implementation for cart management
/// Now uses database storage via CartRepository
/// </summary>
public class CartService(
    ICartRepository cartRepo,
    IBookRepository bookRepo,
    IBookCopyRepository bookCopyRepo,
    IBorrowRequestService borrowRequestService) : ICartService
{
    public async Task<CartDto?> GetCartByAccountIdAsync(Guid accountId)
    {
        // Get cart from database
        var cart = await cartRepo.GetByAccountIdAsync(accountId);

        if (cart == null)
        {
            // Create new empty cart if doesn't exist
            cart = new Entities.Cart
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            cart = await cartRepo.CreateAsync(cart);
        }

        return MapToDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(Guid accountId, Guid bookId)
    {
        // 1. Validate book exists
        var bookExists = await bookRepo.IsBookExistsByIdAsync(bookId);
        if (!bookExists)
            throw new Exception($"Book with ID {bookId} not found.");

        // 2. Check if book has available copies
        var hasAvailableCopies = await bookCopyRepo.HasAvailableCopiesForBook(bookId);
        if (!hasAvailableCopies)
            throw new Exception($"Book with ID {bookId} has no available copies.");

        // 3. Get or create cart
        var cart = await cartRepo.GetByAccountIdAsync(accountId);
        if (cart == null)
        {
            cart = new Entities.Cart
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            cart = await cartRepo.CreateAsync(cart);
        }

        // 4. Check if book already in cart
        var itemExists = await cartRepo.CartItemExistsAsync(cart.Id, bookId);
        if (itemExists)
            throw new Exception("This book is already in your cart.");

        // 5. Add book to cart
        var cartItem = new Entities.CartItem
        {
            Id = Guid.NewGuid(),
            CartId = cart.Id,
            BookId = bookId,
            AddedAt = DateTime.UtcNow
        };

        await cartRepo.AddItemAsync(cartItem);

        // 6. Update cart timestamp
        cart.UpdatedAt = DateTime.UtcNow;
        await cartRepo.UpdateAsync(cart);

        // 7. Reload cart with items
        cart = await cartRepo.GetByAccountIdAsync(accountId);

        return MapToDto(cart!);
    }

    public async Task<bool> RemoveFromCartAsync(Guid accountId, Guid cartItemId)
    {
        // 1. Get cart item to verify ownership
        var cartItem = await cartRepo.GetCartItemByIdAsync(cartItemId);
        if (cartItem == null)
            return false;

        // 2. Verify the cart belongs to the account
        var cart = await cartRepo.GetByAccountIdAsync(accountId);
        if (cart == null || cartItem.CartId != cart.Id)
            return false;

        // 3. Remove the item
        var result = await cartRepo.RemoveItemAsync(cartItemId);

        if (result)
        {
            // 4. Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;
            await cartRepo.UpdateAsync(cart);
        }

        return result;
    }

    public async Task<bool> ClearCartAsync(Guid accountId)
    {
        var cart = await cartRepo.GetByAccountIdAsync(accountId);
        if (cart == null)
            return false;

        var result = await cartRepo.ClearCartAsync(cart.Id);

        if (result)
        {
            // Update cart timestamp
            cart.UpdatedAt = DateTime.UtcNow;
            await cartRepo.UpdateAsync(cart);
        }

        return result;
    }

    public async Task<bool> CreateBorrowRequestFromCartAsync(Guid accountId, string? notes)
    {
        // 1. Get cart
        var cart = await cartRepo.GetByAccountIdAsync(accountId);
        if (cart == null || !cart.Items.Any())
            throw new Exception("Cart is empty.");

        // 2. Get book IDs from cart
        var bookIds = cart.Items.Select(i => i.BookId).ToList();

        // 3. Create borrow request
        var dto = new CreateBorrowRequestDto
        {
            BookIds = bookIds,
            Notes = notes
        };

        await borrowRequestService.CreateBorrowRequestAsync(dto, accountId);

        // 4. Clear cart after successful request creation
        await ClearCartAsync(accountId);

        return true;
    }

    private CartDto MapToDto(Entities.Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            AccountId = cart.AccountId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                BookId = i.BookId,
                BookTitle = i.Book?.Title,
                BookISBN = i.Book?.ISBN,
                BookImageUrl = i.Book?.ImgUrl,
                AddedAt = i.AddedAt,
                IsAvailable = true // Could check actual availability if needed
            }).ToList()
        };
    }
}
