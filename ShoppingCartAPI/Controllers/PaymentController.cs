using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService service)
        {
            _paymentService = service;
        }

        // 9. POST /Purchase
        [Authorize]
        [HttpPost("Purchase")]
        public async Task<IActionResult> Purchase()
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _paymentService.PurchaseAsync(customerId);

            if (!success) return BadRequest("Purchase failed");

            return Ok(new { message = "Purchase completed" });
        }

        // 10. GET /PurchaseHistory
        [Authorize]
        [HttpGet("PurchaseHistory")]
        public async Task<IActionResult> PurchaseHistory()
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var history = await _paymentService.PurchaseHistoryAsync(customerId);
            return Ok(history);
        }
    }
}
