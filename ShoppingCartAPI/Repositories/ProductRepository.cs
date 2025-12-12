using System;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartAPI.Repositories
{
    public class ProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // 2.1 GetProductsbyPrice
        public async Task<List<Product>> GetProductsByPriceAsync(decimal priceMin, decimal priceMax)
        {
            return await _context.Products
                .Where(p => p.Price >= priceMin && p.Price <= priceMax)
                .ToListAsync();
        }

        // 2.2 GetProductsbyCategory
        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();
        }

        // 2.3 GetProductsbyName
        public async Task<List<Product>> GetProductsByNameAsync(string name)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }

        // 2.4 GetAllProducts
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        // 2.5 GetProductById
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }
        public IQueryable<Product> GetQueryable()
        {
            return _context.Products.AsQueryable();
        }

        // 2.6 Update Product Stocks
        public async Task<bool> UpdateProductStock(List<Cart> cartItems) 
        {
            foreach (var cartItem in cartItems)
            {
                cartItem.Product.Stock -= cartItem.Quantity;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
