using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService service)
        {
            _customerService = service;
        }

        // 1. POST /Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            await _customerService.RegisterAsync(dto);            
            return Ok(new { message = "Registered successfully" });
        }

        // 8. PUT /UpdateBudget
        [Authorize]
        [HttpPut("UpdateBudget")]
        public async Task<IActionResult> UpdateBudget([FromBody] UpdateBudgetDto dto)
        {
            int customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _customerService.UpdateBudgetAsync(customerId, dto);
            return Ok(new { message = "Successfully updated the Budget." });
        }
    }
}
