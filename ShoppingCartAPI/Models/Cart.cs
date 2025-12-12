namespace ShoppingCartAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // Foreign Keys
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        // Navigation
        public Customer Customer { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }

}
