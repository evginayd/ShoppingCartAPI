namespace ShoppingCartAPI.Exceptions
{
    public class CustomerAlreadyExistsException : Exception
    {
        public CustomerAlreadyExistsException(string name)
            : base($"A customer with name '{name}' already exists.")
        {
        }
    }
}
