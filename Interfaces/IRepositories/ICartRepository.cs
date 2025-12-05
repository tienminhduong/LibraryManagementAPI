using LibraryManagementAPI.Entities;

namespace LibraryManagementAPI.Interfaces.IRepositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByAccountIdAsync(Guid accountId);
        Task<Cart?> GetByIdAsync(Guid cartId);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart> UpdateAsync(Cart cart);
        Task<bool> DeleteAsync(Guid cartId);
        Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);
        Task<CartItem> AddItemAsync(CartItem cartItem);
        Task<bool> RemoveItemAsync(Guid cartItemId);
        Task<bool> ClearCartAsync(Guid cartId);
        Task<bool> CartItemExistsAsync(Guid cartId, Guid bookId);
    }
}
