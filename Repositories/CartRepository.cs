using LibraryManagementAPI.Context;
using LibraryManagementAPI.Entities;
using LibraryManagementAPI.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Repositories
{
    public class CartRepository(LibraryDbContext dbContext) : ICartRepository
    {
        public async Task<Cart?> GetByAccountIdAsync(Guid accountId)
        {
            return await dbContext.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Book)
                        .ThenInclude(b => b.Authors)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Book)
                        .ThenInclude(b => b.BookCategories)
                .FirstOrDefaultAsync(c => c.AccountId == accountId);
        }

        public async Task<Cart?> GetByIdAsync(Guid cartId)
        {
            return await dbContext.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            await dbContext.Carts.AddAsync(cart);
            await dbContext.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateAsync(Cart cart)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            dbContext.Carts.Update(cart);
            await dbContext.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> DeleteAsync(Guid cartId)
        {
            var cart = await dbContext.Carts.FindAsync(cartId);
            if (cart == null)
                return false;

            dbContext.Carts.Remove(cart);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId)
        {
            return await dbContext.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        public async Task<CartItem> AddItemAsync(CartItem cartItem)
        {
            await dbContext.CartItems.AddAsync(cartItem);
            await dbContext.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> RemoveItemAsync(Guid cartItemId)
        {
            var cartItem = await dbContext.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            dbContext.CartItems.Remove(cartItem);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var cart = await dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                return false;

            dbContext.CartItems.RemoveRange(cart.Items);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CartItemExistsAsync(Guid cartId, Guid bookId)
        {
            return await dbContext.CartItems
                .AnyAsync(ci => ci.CartId == cartId && ci.BookId == bookId);
        }
    }
}
