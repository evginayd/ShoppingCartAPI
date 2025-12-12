namespace ShoppingCartAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public decimal Budget { get; set; }

        // Navigation
        public ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public ICollection<PurchaseHistory> PurchaseHistories { get; set; } = new List<PurchaseHistory>();
    }

}
