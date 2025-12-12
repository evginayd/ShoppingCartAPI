namespace ShoppingCartAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
        public int Stock { get; set; }

        // Navigation
        //public ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        //public ICollection<PurchaseHistory> PurchaseHistories { get; set; } = new List<PurchaseHistory>();
    }

}
