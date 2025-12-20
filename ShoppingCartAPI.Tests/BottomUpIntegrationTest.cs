using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Context;
using ShoppingCartAPI.Controllers;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repositories;
using ShoppingCartAPI.Services;
using Xunit;

namespace ShoppingCartAPI.Tests.IntegrationTesting
{
    public class IntegrationTesting
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        // SESSION 1 – Repository + InMemory DB
        [Fact]
        public async Task S1_ProductRepository_GetProduct_WorksCorrectly()
        {
            var db = GetDbContext();
            // Category eklendi
            db.Products.Add(new Product { Id = 1, Name = "Laptop", Price = 1000, Stock = 10, Category = "Electronics" });
            await db.SaveChangesAsync();

            var repo = new ProductRepository(db);
            var product = await repo.GetProductByIdAsync(1);

            Assert.NotNull(product);
            Assert.Equal("Laptop", product.Name);
        }

        // SESSION 2 – Service + Repository
        [Fact]
        public async Task S2_CartService_AddToCart_UpdatesDatabase()
        {
            var db = GetDbContext();
            // Name ve Category eklendi
            db.Products.Add(new Product { Id = 2, Name = "Mouse", Stock = 10, Price = 100, Category = "Peripherals" });
            await db.SaveChangesAsync();

            var cartRepo = new CartRepository(db);
            var productRepo = new ProductRepository(db);
            var service = new CartService(cartRepo, productRepo);

            await service.AddToCartAsync(1, new AddToCartDto { ProductId = 2, Quantity = 2 });

            var cartItem = await db.Carts.FirstOrDefaultAsync();
            Assert.NotNull(cartItem);
            Assert.Equal(2, cartItem.Quantity);
        }

        // SESSION 3 – Multiple Repositories + Service
        [Fact]
        public async Task S3_PaymentService_Purchase_CreatesHistory()
        {
            var db = GetDbContext();
            // Customer için Name ve Password, Product için Name ve Category eklendi
            db.Customers.Add(new Customer { Id = 1, Name = "TestUser", Password = "123", Budget = 1000 });
            db.Products.Add(new Product { Id = 3, Name = "Keyboard", Price = 100, Stock = 5, Category = "Peripherals" });
            db.Carts.Add(new Cart { CustomerId = 1, ProductId = 3, Quantity = 2 });
            await db.SaveChangesAsync();

            var paymentService = new PaymentService(
                new CartRepository(db),
                new PurchaseHistoryRepository(db),
                new CustomerRepository(db),
                new ProductRepository(db)
            );

            var result = await paymentService.PurchaseAsync(1);

            Assert.True(result);
            Assert.True(await db.PurchaseHistories.AnyAsync());
        }

        // SESSION 4 – Controller + Service + Repository (ClaimsPrincipal Korundu)
        [Fact]
        public async Task S4_CartController_AddToCart_IntegrationWorks()
        {
            var db = GetDbContext();
            // Name ve Category eklendi
            db.Products.Add(new Product { Id = 4, Name = "Monitor", Stock = 10, Price = 50, Category = "Displays" });
            await db.SaveChangesAsync();

            var controller = new CartController(
                new CartService(new CartRepository(db), new ProductRepository(db))
            );

            // CustomerId mantığını korumak için User Claims nesnesini oluşturuyoruz
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var response = await controller.AddToCart(new AddToCartDto { ProductId = 4, Quantity = 1 });

            Assert.IsType<OkObjectResult>(response);
        }

        // SESSION 5 – Full Bottom-Up Validation
        [Fact]
        public async Task S5_FullFlow_AddToCart_To_Purchase_Works()
        {
            var db = GetDbContext();
            // Tüm required alanlar eklendi
            db.Customers.Add(new Customer { Id = 1, Name = "TestUser", Password = "123", Budget = 500 });
            db.Products.Add(new Product { Id = 5, Name = "USB Cable", Price = 50, Stock = 10, Category = "Accessories" });
            await db.SaveChangesAsync();

            var cartService = new CartService(new CartRepository(db), new ProductRepository(db));
            await cartService.AddToCartAsync(1, new AddToCartDto { ProductId = 5, Quantity = 2 });

            var paymentService = new PaymentService(
                new CartRepository(db),
                new PurchaseHistoryRepository(db),
                new CustomerRepository(db),
                new ProductRepository(db)
            );

            var result = await paymentService.PurchaseAsync(1);

            Assert.True(result);
        }
    }
}
