using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repositories;

namespace ShoppingCartAPI.Services
{
    public class PaymentService
    {
        private readonly CartRepository _cartRepository;
        private readonly PurchaseHistoryRepository _historyRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly ProductRepository _productRepository;

        public PaymentService(
            CartRepository cartRepo,
            PurchaseHistoryRepository historyRepo,
            CustomerRepository customerRepo,
            ProductRepository productRepo)
        {
            _cartRepository = cartRepo;
            _historyRepository = historyRepo;
            _customerRepository = customerRepo;
            _productRepository = productRepo;
        }

        // 9. Purchase
        public async Task<bool> PurchaseAsync(int customerId)
        {
            List<Cart> cartItems = await _cartRepository.GetAllCartItemsAsync(customerId);

            if (!cartItems.Any())
                throw new CartEmptyException(customerId);

            decimal total = cartItems.Sum(i => i.Product.Price * i.Quantity);

            Customer? customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new CustomerNotFoundException(customerId);

            if (customer.Budget < total)
                throw new InsufficientBudgetException(total, customer.Budget);

            var insufficientProducts = cartItems
                .Where(ci => ci.Product.Stock < ci.Quantity)
                .ToList();

            if (insufficientProducts.Any())
                throw new InsufficientStockException(insufficientProducts);

            // Payment
            decimal newBudget = customer.Budget - total;
            await _customerRepository.UpdateBudgetAsync(newBudget, customer);

            // Update product stock
            await _productRepository.UpdateProductStock(cartItems);

            // Move cart to history
            await _historyRepository.AddCartToHistoryAsync(customerId);

            // Empty the cart
            await _cartRepository.DeleteCartAsync(customerId);

            return true;
        }


        // 10. PurchaseHistory
        public async Task<List<PurchaseHistory>> PurchaseHistoryAsync(int customerId)
        {
            return await _historyRepository.GetHistoryAsync(customerId);
        }
        
    }

}
