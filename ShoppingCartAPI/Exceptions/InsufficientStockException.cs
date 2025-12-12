using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(IEnumerable<Cart> insufficientItems)
            : base($"Insufficient stock for: {string.Join(", ", insufficientItems.Select(i => i.Product.Name))}")
        {
        }
    }

}
