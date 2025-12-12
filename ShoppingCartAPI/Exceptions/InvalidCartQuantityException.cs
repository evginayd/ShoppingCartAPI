namespace ShoppingCartAPI.Exceptions
{
    public class InvalidCartQuantityException : Exception
    {
        public InvalidCartQuantityException(string message)
            : base(message)
        {
        }
    }
}
