using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repositories;

namespace ShoppingCartAPI.Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepository;
        private readonly ProductRepository _productRepository;

        public CartService(CartRepository repo, ProductRepository productRepo)
        {
            _cartRepository = repo;
            _productRepository = productRepo;
        }

        // 5. AddToCart
        public async Task AddToCartAsync(int customerId, AddToCartDto dto)
        {
            var product = await _productRepository.GetProductByIdAsync(dto.ProductId);
            if (product == null)
                throw new ProductNotFoundException(dto.ProductId);
            if (product.Stock <= 0)
                throw new InvalidQuantityException("Product is out of stock.");
            if (dto.Quantity <= 0)
                throw new InvalidQuantityException("Quantity must be positive.");
            if (product.Stock - dto.Quantity <= 0)
                throw new InvalidQuantityException("Not enough stock for this product.");
            Cart? cartItem = await _cartRepository.GetCartItemByProductId(dto.ProductId, customerId);
            if (cartItem != null)
                await _cartRepository.UpdateProductQuantityAsync(cartItem, dto.Quantity);
            else 
                await _cartRepository.InsertCartItemAsync(customerId, dto.ProductId, dto.Quantity);
        }

        // 6. ViewCart
        public async Task<List<Cart>> ViewCartAsync(int customerId)
        {
            return await _cartRepository.GetAllCartItemsAsync(customerId);
        }

        // 7. DeleteProduct
        public async Task DeleteProductAsync(int cartItemId, int customerId, int quantity)
        {
            Cart? cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
                throw new CartItemNotFoundException(cartItemId);

            if (cartItem.CustomerId != customerId)
                throw new UnauthorizedCartAccessException();

            if (quantity <= 0)
                throw new InvalidCartQuantityException("Quantity must be greater than 0.");

            else if (quantity == cartItem.Quantity)
                await _cartRepository.DeleteCartItemAsync(cartItem);

            else if (quantity > cartItem.Quantity)
                throw new QuantityExceedsCartItemException(quantity, cartItem.Quantity);

            await _cartRepository.UpdateCartItemQuantityAsync(cartItem, quantity);
        }

    }

}
