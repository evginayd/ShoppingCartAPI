using Moq;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Interfaces;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCartAPI.Tests
{
	public class PaymentServiceStatementTests
	{
		private readonly Mock<ICartRepository> _cartRepoMock = new Mock<ICartRepository>();
		private readonly Mock<IPurchaseHistoryRepository> _historyRepoMock = new Mock<IPurchaseHistoryRepository>();
		private readonly Mock<ICustomerRepository> _customerRepoMock = new Mock<ICustomerRepository>();
		private readonly Mock<IProductRepository> _productRepoMock = new Mock<IProductRepository>();
		private readonly PaymentService _service;

		public PaymentServiceStatementTests()
		{
			_service = new PaymentService(
				_cartRepoMock.Object,
				_historyRepoMock.Object,
				_customerRepoMock.Object,
				_productRepoMock.Object);
		}

		// 1. SENARYO: Başarılı Satın Alma (Metodun ana gövdesini ve sonunu kapsar)
		[Fact]
		public async Task PurchaseAsync_Successful_Coverage()
		{
			int customerId = 1;
			var product = new Product { Price = 50, Stock = 10 };
			var cartItems = new List<Cart> { new Cart { Quantity = 2, Product = product } };
			var customer = new Customer { Id = customerId, Budget = 200 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(customerId)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(customerId)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(customerId);

			Assert.True(result);
			_customerRepoMock.Verify(r => r.UpdateBudgetAsync(It.IsAny<decimal>(), customer), Times.Once);
			_productRepoMock.Verify(r => r.UpdateProductStock(cartItems), Times.Once);
		}

		// 2. SENARYO: Boş Sepet Hatası (İlk hata satırını kapsar)
		[Fact]
		public async Task PurchaseAsync_EmptyCart_Coverage()
		{
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(new List<Cart>());
			await Assert.ThrowsAsync<CartEmptyException>(() => _service.PurchaseAsync(1));
		}

		// 3. SENARYO: Müşteri Bulunamadı Hatası (İkinci hata satırını kapsar)
		[Fact]
		public async Task PurchaseAsync_NoCustomer_Coverage()
		{
			var cartItems = new List<Cart> { new Cart { Product = new Product { Price = 10 } } };
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync((Customer)null);

			await Assert.ThrowsAsync<CustomerNotFoundException>(() => _service.PurchaseAsync(1));
		}

		// 4. SENARYO: Yetersiz Bakiye Hatası (Üçüncü hata satırını kapsar)
		[Fact]
		public async Task PurchaseAsync_LowBudget_Coverage()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 100 } } };
			var customer = new Customer { Id = 1, Budget = 20 };
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			await Assert.ThrowsAsync<InsufficientBudgetException>(() => _service.PurchaseAsync(1));
		}

		// 5. SENARYO: Yetersiz Stok Hatası (Dördüncü hata satırını kapsar)
		[Fact]
		public async Task PurchaseAsync_InsufficientStock_Coverage()
		{
			var product = new Product { Price = 10, Stock = 1 };
			var cartItems = new List<Cart> { new Cart { Quantity = 5, Product = product } };
			var customer = new Customer { Id = 1, Budget = 1000 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			await Assert.ThrowsAsync<InsufficientStockException>(() => _service.PurchaseAsync(1));
		}
	}
}