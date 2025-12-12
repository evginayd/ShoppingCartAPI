using Microsoft.EntityFrameworkCore;

using ShoppingCartAPI.Context;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Repositories
{
    public class CartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        // 3.1 InsertCartItem
        public async Task<Cart> InsertCartItemAsync(int customerId, int productId, int quantity)
        {
            var cartItem = new Cart
            {
                CustomerId = customerId,
                ProductId = productId,
                Quantity = quantity
            };

            _context.Carts.Add(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }

        // 3.2 GetAllCartItems
        public async Task<List<Cart>> GetAllCartItemsAsync(int customerId)
        {
            return await _context.Carts
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Product)
                .ToListAsync();
        }

        // 3.3 DeleteCartItem
        public async Task DeleteCartItemAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        // 3.4 DeleteCart (all customer items)
        public async Task DeleteCartAsync(int customerId)
        {
            var items = _context.Carts.Where(c => c.CustomerId == customerId);
            _context.Carts.RemoveRange(items);
            await _context.SaveChangesAsync();
        }


        // 3.7 GetCartItemById
        public async Task<Cart?> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.Carts.FindAsync(cartItemId);
        }

        public async Task UpdateProductQuantityAsync(Cart existingProduct, int quantity)
        {
            existingProduct.Quantity += quantity;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateCartItemQuantityAsync(Cart cartItem, int quantity)
        {
            cartItem.Quantity -= quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Cart?> GetCartItemByProductId(int productId, int customerId)
        {
            return await _context.Carts
                .Where(c => c.ProductId == productId)
                .Where(c => c.CustomerId == customerId)
                .FirstOrDefaultAsync();
        }

    }

}
