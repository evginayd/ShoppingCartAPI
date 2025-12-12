// AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Tables
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -----------------------------
            // CUSTOMER
            // -----------------------------
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.CartItems)
                .WithOne(c => c.Customer)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.PurchaseHistories)
                .WithOne(h => h.Customer)
                .HasForeignKey(h => h.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            /*
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Coupons)
                .WithOne(c => c.Customer)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            */
            modelBuilder.Entity<Customer>()
                .Property(c => c.Budget)
                .HasPrecision(18, 2);

            // -----------------------------
            // PRODUCT
            // -----------------------------
            /*
            modelBuilder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithOne(c => c.Product)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Product>()
                .HasMany(p => p.PurchaseHistories)
                .WithOne(h => h.Product)
                .HasForeignKey(h => h.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            */
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // -----------------------------
            // CART
            // -----------------------------
            modelBuilder.Entity<Cart>()
                .Property(c => c.Quantity)
                .IsRequired();

            // -----------------------------
            // PURCHASE HISTORY
            // -----------------------------
            modelBuilder.Entity<PurchaseHistory>()
                .Property(h => h.Timestamp)
                .HasDefaultValueSql("GETDATE()");

        }
    }
}
