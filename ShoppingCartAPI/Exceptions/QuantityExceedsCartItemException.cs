namespace ShoppingCartAPI.Exceptions
{
    public class QuantityExceedsCartItemException : Exception
    {
        public QuantityExceedsCartItemException(int requested, int available)
            : base($"Cannot remove {requested} items because only {available} are available in the cart.")
        {
        }
    }
}
