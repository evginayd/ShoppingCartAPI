using Moq;
using ShoppingCartAPI.Interfaces;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Tests
{
	public class PaymentServiceLoopTests
	{
		private readonly Mock<ICartRepository> _cartRepoMock = new Mock<ICartRepository>();
		private readonly Mock<IPurchaseHistoryRepository> _historyRepoMock = new Mock<IPurchaseHistoryRepository>();
		private readonly Mock<ICustomerRepository> _customerRepoMock = new Mock<ICustomerRepository>();
		private readonly Mock<IProductRepository> _productRepoMock = new Mock<IProductRepository>();
		private readonly PaymentService _service;

		public PaymentServiceLoopTests()
		{
			_service = new PaymentService(
				_cartRepoMock.Object,
				_historyRepoMock.Object,
				_customerRepoMock.Object,
				_productRepoMock.Object);
		}

		// --- Loop Coverage: Döngünün 1 kez dönmesi (Single Trip) ---
		[Fact]
		public async Task PurchaseAsync_Loop_OneIteration()
		{
			// Arrange
			var cartItems = new List<Cart>
			{
				new Cart { Quantity = 1, Product = new Product { Price = 10, Stock = 10 } }
			};
			var customer = new Customer { Id = 1, Budget = 100 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			// Act
			await _service.PurchaseAsync(1);

			// Assert: Döngü 1 kez çalıştı, stok 10-1=9 olmalı
			Assert.Equal(9, cartItems[0].Product.Stock);
		}

		// --- Loop Coverage: Döngünün birden fazla kez dönmesi (Multiple Trips) ---
		[Fact]
		public async Task PurchaseAsync_Loop_MultipleIterations()
		{
			// Arrange
			var cartItems = new List<Cart>
			{
				new Cart { Quantity = 2, Product = new Product { Price = 10, Stock = 10 } },
				new Cart { Quantity = 3, Product = new Product { Price = 20, Stock = 10 } },
				new Cart { Quantity = 1, Product = new Product { Price = 5, Stock = 5 } }
			};
			var customer = new Customer { Id = 1, Budget = 1000 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			// Act
			await _service.PurchaseAsync(1);

			// Assert: Her ürün için döngü çalıştı ve stoklar düştü mü?
			Assert.Equal(8, cartItems[0].Product.Stock); // 10 - 2
			Assert.Equal(7, cartItems[1].Product.Stock); // 10 - 3
			Assert.Equal(4, cartItems[2].Product.Stock); // 5 - 1
		}
	}
}