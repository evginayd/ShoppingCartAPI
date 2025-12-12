namespace ShoppingCartAPI.Exceptions
{
    public class CartItemNotFoundException : Exception
    {
        public CartItemNotFoundException(int productId)
            : base($"Cart item with product ID {productId} was not found.")
        {
        }
    }
}
