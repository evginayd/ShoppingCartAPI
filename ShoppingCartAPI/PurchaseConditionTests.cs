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
	public class PaymentServiceConditionTests
	{
		private readonly Mock<ICartRepository> _cartRepoMock = new Mock<ICartRepository>();
		private readonly Mock<IPurchaseHistoryRepository> _historyRepoMock = new Mock<IPurchaseHistoryRepository>();
		private readonly Mock<ICustomerRepository> _customerRepoMock = new Mock<ICustomerRepository>();
		private readonly Mock<IProductRepository> _productRepoMock = new Mock<IProductRepository>();
		private readonly PaymentService _service;

		public PaymentServiceConditionTests()
		{
			_service = new PaymentService(
				_cartRepoMock.Object,
				_historyRepoMock.Object,
				_customerRepoMock.Object,
				_productRepoMock.Object);
		}

		// --- Condition Coverage: if (A && B) ---

		[Fact]
		public async Task Condition_A_True_B_True_Throws()
		{
			// A (Budget < total): 50 < 100 (True)
			// B (total > 0): 100 > 0 (True)
			// Sonuç: True && True = True (İçeri girer, hata fırlatır)
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 100, Stock = 10 } } };
			var customer = new Customer { Budget = 50 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			await Assert.ThrowsAsync<InsufficientBudgetException>(() => _service.PurchaseAsync(1));
		}

		[Fact]
		public async Task Condition_A_False_B_True_Success()
		{
			// A (Budget < total): 150 < 100 (False)
			// B (total > 0): 100 > 0 (True)
			// Sonuç: False && True = False (İçeri girmez, başarılı biter)
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 100, Stock = 10 } } };
			var customer = new Customer { Budget = 150 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(1);
			Assert.True(result);
		}

		[Fact]
		public async Task Condition_A_True_B_False_Success()
		{
			// A (Budget < total): -10 < 0 (True) -> (Negatif bütçe simülasyonu)
			// B (total > 0): 0 > 0 (False)
			// Sonuç: True && False = False (İçeri girmez, başarılı biter)
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 0, Stock = 10 } } };
			var customer = new Customer { Budget = -10 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(1);
			Assert.True(result);
		}

		[Fact]
		public async Task Condition_A_False_B_False_Success()
		{
			// A (Budget < total): 10 < 0 (False)
			// B (total > 0): 0 > 0 (False)
			// Sonuç: False && False = False (İçeri girmez, başarılı biter)
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 0, Stock = 10 } } };
			var customer = new Customer { Budget = 10 };

			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(1);
			Assert.True(result);
		}
	}
}