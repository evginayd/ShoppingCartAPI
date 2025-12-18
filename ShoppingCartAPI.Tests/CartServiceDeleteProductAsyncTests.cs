using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Interfaces;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services;
using Xunit;

namespace ShoppingCartAPI.Tests
{
    public class CartServiceDeleteProductAsyncTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly CartService _cartService;

        public CartServiceDeleteProductAsyncTests()
        {
            _cartRepoMock = new Mock<ICartRepository>();
            _productRepoMock = new Mock<IProductRepository>();

            // Your CartService likely requires both repositories in the constructor
            _cartService = new CartService(
                _cartRepoMock.Object,
                _productRepoMock.Object
            );
        }

        // P2 – Cart item not found
        [Fact]
        public async Task DeleteProductAsync_WhenItemNotFound_ThrowsCartItemNotFoundException()
        {
            _cartRepoMock
                .Setup(r => r.GetCartItemByIdAsync(99))
                .ReturnsAsync((Cart)null);

            await Assert.ThrowsAsync<CartItemNotFoundException>(() =>
                _cartService.DeleteProductAsync(99, 1, 1));
        }

        // P3 – Unauthorized access (Wrong customer)
        [Fact]
        public async Task DeleteProductAsync_WhenWrongCustomer_ThrowsUnauthorizedCartAccessException()
        {
            var cartItem = new Cart { Id = 1, CustomerId = 10, Quantity = 5 };

            _cartRepoMock
                .Setup(r => r.GetCartItemByIdAsync(1))
                .ReturnsAsync(cartItem);

            await Assert.ThrowsAsync<UnauthorizedCartAccessException>(() =>
                _cartService.DeleteProductAsync(1, 99, 1));
        }

        // P4 – Quantity <= 0
        [Fact]
        public async Task DeleteProductAsync_WhenQuantityIsZeroOrNegative_ThrowsInvalidCartQuantityException()
        {
            var cartItem = new Cart { Id = 1, CustomerId = 10, Quantity = 5 };

            _cartRepoMock
                .Setup(r => r.GetCartItemByIdAsync(1))
                .ReturnsAsync(cartItem);

            await Assert.ThrowsAsync<InvalidCartQuantityException>(() =>
                _cartService.DeleteProductAsync(1, 10, 0));
        }

        // P5 – Quantity matches exactly -> Delete item
        [Fact]
        public async Task DeleteProductAsync_WhenQuantityMatches_DeletesCartItem()
        {
            var cartItem = new Cart { Id = 1, CustomerId = 10, Quantity = 5 };

            _cartRepoMock
                .Setup(r => r.GetCartItemByIdAsync(1))
                .ReturnsAsync(cartItem);

            await _cartService.DeleteProductAsync(1, 10, 5);

            _cartRepoMock.Verify(
                r => r.DeleteCartItemAsync(cartItem),
                Times.Once);
        }

        // P6 – Quantity exceeds available
        [Fact]
        public async Task DeleteProductAsync_WhenQuantityExceedsAvailable_ThrowsQuantityExceedsCartItemException()
        {
            var cartItem = new Cart { Id = 1, CustomerId = 10, Quantity = 5 };

            _cartRepoMock.Setup(r => r.GetCartItemByIdAsync(1)).ReturnsAsync(cartItem);

            await Assert.ThrowsAsync<QuantityExceedsCartItemException>(() =>
                _cartService.DeleteProductAsync(1, 10, 10));
        }

        // P1 – Valid quantity less than total -> Update (Decrease)
        [Fact]
        public async Task DeleteProductAsync_WhenQuantityIsValid_UpdatesQuantity()
        {
            var cartItem = new Cart { Id = 1, CustomerId = 10, Quantity = 5 };

            _cartRepoMock
                .Setup(r => r.GetCartItemByIdAsync(1))
                .ReturnsAsync(cartItem);

            await _cartService.DeleteProductAsync(1, 10, 2);

            _cartRepoMock.Verify(
                r => r.UpdateCartItemQuantityAsync(cartItem, 2),
                Times.Once);
        }
    }
}
