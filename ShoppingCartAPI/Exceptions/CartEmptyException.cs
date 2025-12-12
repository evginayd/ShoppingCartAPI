namespace ShoppingCartAPI.Exceptions
{
    public class CartEmptyException : Exception
    {
        public CartEmptyException(int customerId)
            : base($"Cart is empty for customer with id {customerId}.")
        {
        }
    }
}
