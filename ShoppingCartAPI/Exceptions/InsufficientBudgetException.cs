namespace ShoppingCartAPI.Exceptions
{
    public class InsufficientBudgetException : Exception
    {
        public InsufficientBudgetException(decimal required, decimal available)
            : base($"Not enough budget. Required: {required}, Available: {available}")
        {
        }
    }

}
