using Moq;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Interfaces;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services;
using Xunit;

namespace ShoppingCartAPI.Tests
{
	public class PaymentServiceBranchTests
	{
		private readonly Mock<ICartRepository> _cartRepoMock = new Mock<ICartRepository>();
		private readonly Mock<IPurchaseHistoryRepository> _historyRepoMock = new Mock<IPurchaseHistoryRepository>();
		private readonly Mock<ICustomerRepository> _customerRepoMock = new Mock<ICustomerRepository>();
		private readonly Mock<IProductRepository> _productRepoMock = new Mock<IProductRepository>();
		private readonly PaymentService _service;

		public PaymentServiceBranchTests()
		{
			_service = new PaymentService(_cartRepoMock.Object, _historyRepoMock.Object, _customerRepoMock.Object, _productRepoMock.Object);
		}

		// Branch: If'lerin 'False' döndüğü ve metodun tamamlandığı ana yol
		[Fact]
		public async Task PurchaseAsync_Branch_AllFalse_Successful()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 10, Stock = 10 } } };
			var customer = new Customer { Id = 1, Budget = 100 };
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(1);
			Assert.True(result);
		}

		// Branch: !cartItems.Any() == True
		[Fact]
		public async Task PurchaseAsync_Branch_CartEmpty_True()
		{
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(new List<Cart>());
			await Assert.ThrowsAsync<CartEmptyException>(() => _service.PurchaseAsync(1));
		}

		// Branch: customer == null == True
		[Fact]
		public async Task PurchaseAsync_Branch_CustomerNull_True()
		{
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(new List<Cart> { new Cart { Product = new Product() } });
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync((Customer)null);
			await Assert.ThrowsAsync<CustomerNotFoundException>(() => _service.PurchaseAsync(1));
		}

		// Branch: Budget < total && total > 0 == True
		[Fact]
		public async Task PurchaseAsync_Branch_InsufficientBudget_True()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 100 } } };
			var customer = new Customer { Budget = 50 };
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
			await Assert.ThrowsAsync<InsufficientBudgetException>(() => _service.PurchaseAsync(1));
		}

		// Branch: total > 0 == False (Branch Coverage için kritik ekleme)
		[Fact]
		public async Task PurchaseAsync_Branch_TotalIsZero_False()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 1, Product = new Product { Price = 0, Stock = 10 } } };
			var customer = new Customer { Budget = 0 };
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);

			var result = await _service.PurchaseAsync(1);
			Assert.True(result); // Tutar 0 olduğu için bütçe hatası vermemeli
		}

		// Branch: insufficientProducts.Any() == True
		[Fact]
		public async Task PurchaseAsync_Branch_InsufficientStock_True()
		{
			var cartItems = new List<Cart> { new Cart { Quantity = 10, Product = new Product { Stock = 2, Price = 1 } } };
			var customer = new Customer { Budget = 100 };
			_cartRepoMock.Setup(r => r.GetAllCartItemsAsync(1)).ReturnsAsync(cartItems);
			_customerRepoMock.Setup(r => r.GetCustomerByIdAsync(1)).ReturnsAsync(customer);
			await Assert.ThrowsAsync<InsufficientStockException>(() => _service.PurchaseAsync(1));
		}
	}
}