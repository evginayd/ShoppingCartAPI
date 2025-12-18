using Moq;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Interfaces;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services;
using Xunit;


namespace ShoppingCartAPI.Tests
{
    public class CartServiceAddToCartAsyncTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly CartService _cartService;

        public CartServiceAddToCartAsyncTests()
        {
            _cartRepoMock = new Mock<ICartRepository>();
            _productRepoMock = new Mock<IProductRepository>();
            _cartService = new CartService(
                _cartRepoMock.Object,
                _productRepoMock.Object
            );
        }

        // ─────────────────────────────────────────────
        // P1 – Product not found
        // Covers: Statement, Branch
        // ─────────────────────────────────────────────
        [Fact]
        public async Task AddToCartAsync_WhenProductNotFound_ThrowsProductNotFoundException()
        {
            _productRepoMock
                .Setup(p => p.GetProductByIdAsync(1))
                .ReturnsAsync((Product)null);

            var dto = new AddToCartDto { ProductId = 1, Quantity = 1 };

            await Assert.ThrowsAsync<ProductNotFoundException>(() =>
                _cartService.AddToCartAsync(1, dto));
        }

        // ─────────────────────────────────────────────
        // P2 – Product out of stock (Stock <= 0 && Quantity > 0)
        // Covers: Branch, Condition, MCDC
        // ─────────────────────────────────────────────
        [Fact]
        public async Task AddToCartAsync_WhenProductOutOfStock_ThrowsInvalidQuantityException()
        {
            var product = new Product { Id = 1, Stock = 0 };

            _productRepoMock
                .Setup(p => p.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            var dto = new AddToCartDto { ProductId = 1, Quantity = 1 };

            await Assert.ThrowsAsync<InvalidQuantityException>(() =>
                _cartService.AddToCartAsync(1, dto));
        }

        // ─────────────────────────────────────────────
        // P3 – Quantity <= 0
        // Covers: Statement, Branch
        // ─────────────────────────────────────────────
        [Fact]
        public async Task AddToCartAsync_WhenQuantityIsZero_ThrowsInvalidQuantityException()
        {
            var product = new Product { Id = 1, Stock = 10 };

            _productRepoMock
                .Setup(p => p.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            var dto = new AddToCartDto { ProductId = 1, Quantity = 0 };

            await Assert.ThrowsAsync<InvalidQuantityException>(() =>
                _cartService.AddToCartAsync(1, dto));
        }

        // ─────────────────────────────────────────────
        // P4 – Not enough stock (Stock - Quantity <= 0)
        // Covers: Branch, Condition
        // ─────────────────────────────────────────────
        [Fact]
        public async Task AddToCartAsync_WhenNotEnoughStock_ThrowsInvalidQuantityException()
        {
            var product = new Product { Id = 1, Stock = 5 };

            _productRepoMock
                .Setup(p => p.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            var dto = new AddToCartDto { ProductId = 1, Quantity = 5 };

            await Assert.ThrowsAsync<InvalidQuantityException>(() =>
                _cartService.AddToCartAsync(1, dto));
        }

        // ─────────────────────────────────────────────
        // P5 – Cart item exists → update quantity
        // Covers: Statement, Branch
        // ─────────────────────────────────────────────
        [Fact]
        public async Task AddToCartAsync_WhenCartItemExists_UpdatesQuantity()
        {
            var product = new Product { Id = 1, Stock = 10 };
            var cartItem = new Cart { ProductId = 1, Quantity = 2, CustomerId = 1 };

            _productRepoMock
                .Setup(p => p.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _cartRepoMock
                .Setup(c => c.GetCartItemByProductId(1, 1))
                .ReturnsAsync(cartItem);

            var dto = new AddToCartDto { ProductId = 1, Quantity = 2 };

            await _cartService.AddToCartAsync(1, dto);

            _cartRepoMock.Verify(
                c => c.UpdateProductQuantityAsync(cartItem, 2),
                Times.Once);
        }

        // ─────────────────────────────────────────────
        // P6 – Cart item does not exist → insert
        // Covers: Statement, Branch
        // ─────────────────────────────────────────────
        [Fact]
        public async Task AddToCartAsync_WhenCartItemDoesNotExist_InsertsNewCartItem()
        {
            var product = new Product { Id = 1, Stock = 10 };

            _productRepoMock
                .Setup(p => p.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            _cartRepoMock
                .Setup(c => c.GetCartItemByProductId(1, 1))
                .ReturnsAsync((Cart)null);

            var dto = new AddToCartDto { ProductId = 1, Quantity = 2 };

            await _cartService.AddToCartAsync(1, dto);

            _cartRepoMock.Verify(
                c => c.InsertCartItemAsync(1, 1, 2),
                Times.Once);
        }
    }
}