using Moq;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Interfaces;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services;
using Xunit;

public class CartServiceTests
{
	private readonly Mock<ICartRepository> _cartRepoMock;
	private readonly Mock<IProductRepository> _productRepoMock;
	private readonly CartService _cartService;

	public CartServiceTests()
	{
		_cartRepoMock = new Mock<ICartRepository>();
		_productRepoMock = new Mock<IProductRepository>();
		_cartService = new CartService(_cartRepoMock.Object, _productRepoMock.Object);
	}

	[Fact]
	public async Task AddToCartAsync_Throws_WhenProductNotFound()
	{
		_productRepoMock
			.Setup(p => p.GetProductByIdAsync(It.IsAny<int>()))
			.ReturnsAsync((Product)null);

		var dto = new AddToCartDto { ProductId = 1, Quantity = 1 };

		await Assert.ThrowsAsync<ProductNotFoundException>(() =>
			_cartService.AddToCartAsync(1, dto));
	}

	[Fact]
	public async Task AddToCartAsync_Throws_WhenOutOfStock()
	{
		var product = new Product { Id = 1, Stock = 0 };

		_productRepoMock
			.Setup(p => p.GetProductByIdAsync(1))
			.ReturnsAsync(product);

		var dto = new AddToCartDto { ProductId = 1, Quantity = 1 };

		await Assert.ThrowsAsync<InvalidQuantityException>(() =>
			_cartService.AddToCartAsync(1, dto));
	}

	[Fact]
	public async Task AddToCartAsync_Throws_WhenQuantityIsZero()
	{
		var product = new Product { Id = 1, Stock = 10 };

		_productRepoMock
			.Setup(p => p.GetProductByIdAsync(1))
			.ReturnsAsync(product);

		var dto = new AddToCartDto { ProductId = 1, Quantity = 0 };

		await Assert.ThrowsAsync<InvalidQuantityException>(() =>
			_cartService.AddToCartAsync(1, dto));
	}

	[Fact]
	public async Task AddToCartAsync_Throws_WhenNotEnoughStock()
	{
		var product = new Product { Id = 1, Stock = 5 };

		_productRepoMock
			.Setup(p => p.GetProductByIdAsync(1))
			.ReturnsAsync(product);

		var dto = new AddToCartDto { ProductId = 1, Quantity = 5 };

		await Assert.ThrowsAsync<InvalidQuantityException>(() =>
			_cartService.AddToCartAsync(1, dto));
	}

	[Fact]
	public async Task AddToCartAsync_Updates_WhenCartItemExists()
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

	[Fact]
	public async Task AddToCartAsync_Inserts_WhenCartItemDoesNotExist()
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
