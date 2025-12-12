using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService service)
        {
            _cartService = service;
        }

        // 5. POST /AddToCart
        [Authorize]
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _cartService.AddToCartAsync(customerId, dto);
            return Ok(new { message = "Product successfully added to the Cart." });
        }

        // 6. GET /ViewCart
        [Authorize]
        [HttpGet("ViewCart")]
        public async Task<IActionResult> ViewCart()
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var items = await _cartService.ViewCartAsync(customerId);
            return Ok(items);
        }

        // 7. DELETE /DeleteFromCart?CartItemId=123Quantity=1
        [Authorize]
        [HttpDelete("DeleteFromCart")]
        public async Task<IActionResult> DeleteFromCart([FromQuery] int CartItemId, [FromQuery] int quantity)
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _cartService.DeleteProductAsync(CartItemId, customerId, quantity);

            return Ok(new { message = "Porduct successful deleted from the Cart." });
        }

    }
}
