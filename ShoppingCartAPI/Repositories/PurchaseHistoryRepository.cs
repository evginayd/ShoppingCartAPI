using System;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartAPI.Repositories
{
    public class PurchaseHistoryRepository
    {
        private readonly AppDbContext _context;

        public PurchaseHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        // 4.1 AddCartToHistory
        public async Task AddCartToHistoryAsync(int customerId)
        {
            var cartItems = await _context.Carts
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Product)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                var history = new PurchaseHistory
                {
                    CustomerId = customerId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Timestamp = DateTime.Now
                };

                _context.PurchaseHistories.Add(history);
            }

            // Clear cart
            //_context.Carts.RemoveRange(cartItems);

            await _context.SaveChangesAsync();
        }

        // 4.2 GetHistory
        public async Task<List<PurchaseHistory>> GetHistoryAsync(int customerId)
        {
            return await _context.PurchaseHistories
                .Where(h => h.CustomerId == customerId)
                .Include(h => h.Product)
                .OrderByDescending(h => h.Timestamp)
                .ToListAsync();
        }

    }

}
