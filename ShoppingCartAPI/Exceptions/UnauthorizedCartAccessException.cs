namespace ShoppingCartAPI.Exceptions
{
    public class UnauthorizedCartAccessException : Exception
    {
        public UnauthorizedCartAccessException()
            : base("You are not allowed to access this cart item.")
        {
        }
    }
}
