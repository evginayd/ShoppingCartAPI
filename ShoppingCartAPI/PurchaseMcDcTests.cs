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
	public class PaymentServiceMCDCTests
	{
		private readonly Mock<ICartRepository> _cartRepoMock = new Mock<ICartRepository>();
		private readonly Mock<IPurchaseHistoryRepository> _historyRepoMock = new Mock<IPurchaseHistoryRepository>();
		private readonly Mock<ICustomerRepository> _customerRepoMock = new Mock<ICustomerRepository>();
		private readonly Mock<IProductRepository> _productRepoMock = new Mock<IProductRepository>();
		private readonly PaymentService _service;

		public PaymentServiceMCDCTests()
		{
			_service = new PaymentService(
				_cartRepoMock.Object,
				_historyRepoMock.Object,
				_customerRepoMock.Object,
				_productRepoMock.Object);
		}

		// --- MCDC Durum 1: (A: True, B: True) -> Sonuç: True ---
		[Fact]
		public async Task MCDC_Case1_BudgetLow_And_TotalPositive_Throws()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 100, Stock = 10 } } };
			var customer = new Customer { Budget = 50 }; // 50 < 100 (T) ve 100 > 0 (T)

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			await Assert.ThrowsAsync<InsufficientBudgetException>(() => _service.PurchaseAsync(1));
		}

		// --- MCDC Durum 2: (A: False, B: True) -> Sonuç: False ---
		// A değiştiğinde sonucun değiştiğini kanıtlar.
		[Fact]
		public async Task MCDC_Case2_BudgetEnough_And_TotalPositive_Success()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 100, Stock = 10 } } };
			var customer = new Customer { Budget = 150 }; // 150 < 100 (F) ve 100 > 0 (T)

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(1);
			Assert.True(result);
		}

		// --- MCDC Durum 3: (A: True, B: False) -> Sonuç: False ---
		// B değiştiğinde sonucun değiştiğini kanıtlar.
		[Fact]
		public async Task MCDC_Case3_BudgetLow_And_TotalZero_Success()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 0, Stock = 10 } } };
			var customer = new Customer { Budget = -10 }; // -10 < 0 (T) ve 0 > 0 (F)

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(1);
			Assert.True(result);
		}
	}
}