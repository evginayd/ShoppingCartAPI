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

namespace ShoppingCartAPI.Tests
{
    public class PaymentServicePurchaseAsyncTests
    {
        private readonly Mock<ICartRepository> _cartRepoMock;
        private readonly Mock<IPurchaseHistoryRepository> _historyRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly PaymentService _service;

        public PaymentServicePurchaseAsyncTests()
        {
            _cartRepoMock = new Mock<ICartRepository>();
            _historyRepoMock = new Mock<IPurchaseHistoryRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _productRepoMock = new Mock<IProductRepository>();

            _service = new PaymentService(
                _cartRepoMock.Object,
                _historyRepoMock.Object,
                _customerRepoMock.Object,
                _productRepoMock.Object
            );
        }

        // ─────────────────────────────────────────────
        // P1 – Cart Empty → Exception
        // Covers: Statement, Branch
        // ─────────────────────────────────────────────
        [Fact]
        public async Task PurchaseAsync_WhenCartEmpty_ThrowsCartEmptyException()
        {
            _cartRepoMock
                .Setup(r => r.GetAllCartItemsAsync(1))
                .ReturnsAsync(new List<Cart>());

            await Assert.ThrowsAsync<CartEmptyException>(() =>
                _service.PurchaseAsync(1));
        }

        // ─────────────────────────────────────────────
        // P2 – Customer Not Found → Exception
        // Covers: Statement, Branch
        // ─────────────────────────────────────────────
        [Fact]
        public async Task PurchaseAsync_WhenCustomerNotFound_ThrowsCustomerNotFoundException()
        {
            var cartItems = new List<Cart>
            {
                new Cart { Quantity = 1, Product = new Product { Price = 10, Stock = 10 } }
            };

            _cartRepoMock
                .Setup(r => r.GetAllCartItemsAsync(1))
                .ReturnsAsync(cartItems);

            _customerRepoMock
                .Setup(r => r.GetCustomerByIdAsync(1))
                .ReturnsAsync((Customer)null);

            await Assert.ThrowsAsync<CustomerNotFoundException>(() =>
                _service.PurchaseAsync(1));
        }

        // ─────────────────────────────────────────────
        // P3 – Insufficient Budget (A=T, B=T)
        // Covers: Branch, Condition, MCDC
        // ─────────────────────────────────────────────
        [Fact]
        public async Task PurchaseAsync_WhenBudgetInsufficient_ThrowsInsufficientBudgetException()
        {
            var cartItems = new List<Cart>
            {
                new Cart { Quantity = 1, Product = new Product { Price = 100, Stock = 10 } }
            };

            var customer = new Customer { Id = 1, Budget = 50 };

            _cartRepoMock
                .Setup(r => r.GetAllCartItemsAsync(1))
                .ReturnsAsync(cartItems);

            _customerRepoMock
                .Setup(r => r.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            await Assert.ThrowsAsync<InsufficientBudgetException>(() =>
                _service.PurchaseAsync(1));
        }

        // ─────────────────────────────────────────────
        // P4 – Insufficient Stock → Exception
        // Covers: Branch, Condition
        // ─────────────────────────────────────────────
        [Fact]
        public async Task PurchaseAsync_WhenStockInsufficient_ThrowsInsufficientStockException()
        {
            var cartItems = new List<Cart>
            {
                new Cart { Quantity = 10, Product = new Product { Price = 10, Stock = 2 } }
            };

            var customer = new Customer { Id = 1, Budget = 500 };

            _cartRepoMock
                .Setup(r => r.GetAllCartItemsAsync(1))
                .ReturnsAsync(cartItems);

            _customerRepoMock
                .Setup(r => r.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            await Assert.ThrowsAsync<InsufficientStockException>(() =>
                _service.PurchaseAsync(1));
        }

        // ─────────────────────────────────────────────
        // P5 – Successful Purchase (Single Iteration)
        // Covers: Statement, Branch, Loop (1 iteration)
        // ─────────────────────────────────────────────
        [Fact]
        public async Task PurchaseAsync_SuccessfulPurchase_SingleItem()
        {
            var cartItems = new List<Cart>
            {
                new Cart { Quantity = 2, Product = new Product { Price = 20, Stock = 10 } }
            };

            var customer = new Customer { Id = 1, Budget = 100 };

            _cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1))
                .ReturnsAsync(cartItems);

            _customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            var result = await _service.PurchaseAsync(1);

            Assert.True(result);
            Assert.Equal(8, cartItems[0].Product.Stock);

            _customerRepoMock.Verify(
                r => r.UpdateBudgetAsync(It.IsAny<decimal>(), customer),
                Times.Once);
        }

        // ─────────────────────────────────────────────
        // P6 – Successful Purchase (Multiple Iterations)
        // Covers: Statement, Branch, Loop (n iterations)
        // ─────────────────────────────────────────────
        [Fact]
        public async Task PurchaseAsync_SuccessfulPurchase_MultipleItems()
        {
            var cartItems = new List<Cart>
            {
                new Cart { Quantity = 1, Product = new Product { Price = 10, Stock = 5 } },
                new Cart { Quantity = 2, Product = new Product { Price = 20, Stock = 10 } }
            };

            var customer = new Customer { Id = 1, Budget = 200 };

            _cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1))
                .ReturnsAsync(cartItems);

            _customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            var result = await _service.PurchaseAsync(1);

            Assert.True(result);
            Assert.Equal(4, cartItems[0].Product.Stock);
            Assert.Equal(8, cartItems[1].Product.Stock);
        }
    }
}
