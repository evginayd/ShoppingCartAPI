// Seed.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Context;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI
{
    public class Seed
    {
        private readonly AppDbContext _context;

        public Seed(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            // DB varsa migration’ları uygula
            await _context.Database.MigrateAsync();

            // -----------------------------
            // 1️⃣ CUSTOMER
            // -----------------------------
            if (!await _context.Customers.AnyAsync())
            {
                var customers = new List<Customer>
                {
                    new Customer { Name = "evgin", Password = "1234", Budget = 500M },
                    new Customer { Name = "ayse", Password = "pass", Budget = 200M },
                    new Customer { Name = "mehmet", Password = "qwerty", Budget = 1000M }
                };

                await _context.Customers.AddRangeAsync(customers);
                await _context.SaveChangesAsync();
            }

            // -----------------------------
            // 2️⃣ PRODUCT
            // -----------------------------
            if (!await _context.Products.AnyAsync())
            {
                var products = new List<Product>
                {
                    new Product { Name = "Laptop", Price = 15000M, Category = "Electronics", Stock = 10 },
                    new Product { Name = "T-Shirt", Price = 250M, Category = "Clothes", Stock = 50 },
                    new Product { Name = "Shoes", Price = 800M, Category = "Clothes", Stock = 20 }
                };

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
            }

            // -----------------------------
            // 4️⃣ CART
            // -----------------------------
            if (!await _context.Carts.AnyAsync())
            {
                var carts = new List<Cart>();
                await _context.Carts.AddRangeAsync(carts);
                await _context.SaveChangesAsync();
            }

            // -----------------------------
            // 5️⃣ PURCHASE HISTORY
            // -----------------------------
            if (!await _context.PurchaseHistories.AnyAsync())
            {
                var history = new List<PurchaseHistory>();
                await _context.PurchaseHistories.AddRangeAsync(history);
                await _context.SaveChangesAsync();
            }
        }
    }
}
