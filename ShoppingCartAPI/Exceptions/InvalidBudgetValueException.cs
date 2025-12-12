namespace ShoppingCartAPI.Exceptions
{
    public class InvalidBudgetValueException : Exception
    {
        public InvalidBudgetValueException(decimal value)
            : base($"Budget value '{value}' is invalid. Budget must be greater than or equal to 0.")
        {
        }
    }
}
