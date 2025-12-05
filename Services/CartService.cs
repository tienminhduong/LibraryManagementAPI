using LibraryManagementAPI.Interfaces.IRepositories;
using LibraryManagementAPI.Interfaces.IServices;
using LibraryManagementAPI.Models.BorrowRequest;
using LibraryManagementAPI.Models.Cart;

namespace LibraryManagementAPI.Services;

/// <summary>
/// Service implementation for cart management
/// Uses in-memory storage for simplicity (could be moved to database)
/// </summary>
public class CartService(
    IInfoRepository infoRepo,
    IBookRepository bookRepo,
    IBookCopyRepository bookCopyRepo,
    IBorrowRequestService borrowRequestService) : ICartService
{
    // In-memory storage for carts (keyed by MemberId)
    // In production, this should be stored in database or distributed cache (Redis)
    private static readonly Dictionary<Guid, Cart> _carts = new();
    private static readonly object _lock = new();

    public async Task<CartDto?> GetCartByAccountIdAsync(Guid accountId)
    {
        // Get member info from account ID
        var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
        if (memberInfo == null || memberInfo is not Entities.MemberInfo)
            throw new Exception("Member information not found.");

        var memberId = memberInfo.id;

        lock (_lock)
        {
            if (!_carts.TryGetValue(memberId, out var cart))
            {
                // Create new empty cart if doesn't exist
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    MemberId = memberId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _carts[memberId] = cart;
            }

            return MapToDto(cart);
        }
    }

    public async Task<CartDto> AddToCartAsync(Guid accountId, Guid bookId)
    {
        // 1. Get member info
        var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
        if (memberInfo == null || memberInfo is not Entities.MemberInfo)
            throw new Exception("Member information not found.");

        var memberId = memberInfo.id;

        // 2. Validate book exists
        var bookExists = await bookRepo.IsBookExistsByIdAsync(bookId);
        if (!bookExists)
            throw new Exception($"Book with ID {bookId} not found.");

        // 3. Check if book has available copies
        var hasAvailableCopies = await bookCopyRepo.HasAvailableCopiesForBook(bookId);
        if (!hasAvailableCopies)
            throw new Exception($"Book with ID {bookId} has no available copies.");

        lock (_lock)
        {
            // Get or create cart
            if (!_carts.TryGetValue(memberId, out var cart))
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    MemberId = memberId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _carts[memberId] = cart;
            }

            // Check if book already in cart
            if (cart.Items.Any(i => i.BookId == bookId))
                throw new Exception("This book is already in your cart.");

            // Add book to cart
            cart.Items.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                BookId = bookId,
                AddedAt = DateTime.UtcNow
            });

            cart.UpdatedAt = DateTime.UtcNow;

            return MapToDto(cart);
        }
    }

    public async Task<bool> RemoveFromCartAsync(Guid accountId, Guid cartItemId)
    {
        var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
        if (memberInfo == null || memberInfo is not Entities.MemberInfo)
            throw new Exception("Member information not found.");

        var memberId = memberInfo.id;

        lock (_lock)
        {
            if (!_carts.TryGetValue(memberId, out var cart))
                return false;

            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null)
                return false;

            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;

            return true;
        }
    }

    public async Task<bool> ClearCartAsync(Guid accountId)
    {
        var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
        if (memberInfo == null || memberInfo is not Entities.MemberInfo)
            throw new Exception("Member information not found.");

        var memberId = memberInfo.id;

        lock (_lock)
        {
            if (!_carts.TryGetValue(memberId, out var cart))
                return false;

            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;

            return true;
        }
    }

    public async Task<bool> CreateBorrowRequestFromCartAsync(Guid accountId, string? notes)
    {
        var memberInfo = await infoRepo.GetByAccountIdAsync(accountId);
        if (memberInfo == null || memberInfo is not Entities.MemberInfo)
            throw new Exception("Member information not found.");

        var memberId = memberInfo.id;

        List<Guid> bookIds;

        lock (_lock)
        {
            if (!_carts.TryGetValue(memberId, out var cart) || !cart.Items.Any())
                throw new Exception("Cart is empty.");

            // Get book IDs from cart
            bookIds = cart.Items.Select(i => i.BookId).ToList();
        }

        // Create borrow request
        var dto = new CreateBorrowRequestDto
        {
            BookIds = bookIds,
            Notes = notes
        };

        await borrowRequestService.CreateBorrowRequestAsync(dto, accountId);

        // Clear cart after successful request creation
        await ClearCartAsync(accountId);

        return true;
    }

    private CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            MemberId = cart.MemberId,
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                BookId = i.BookId,
                AddedAt = i.AddedAt
                // Book details would be loaded separately or via join
            }).ToList()
        };
    }
}
