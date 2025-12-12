using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService service)
        {
            _productService = service;
        }

        // 3. GET /Home
        [HttpGet("Home")]
        public async Task<IActionResult> Home()
        {
            var products = await _productService.HomeAsync();
            return Ok(products);
        }

        // 4. GET /Products?priceMin=10&priceMax=100&category=dress&name=abc
        [HttpGet("Products")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] decimal? priceMin,
            [FromQuery] decimal? priceMax,
            [FromQuery] string? category,
            [FromQuery] string? name)
        {
            var dto = new FilterProductDto
            {
                PriceMin = priceMin,
                PriceMax = priceMax,
                Category = category,
                Name = name
            };

            var products = await _productService.FilterProductsAsync(dto);
            return Ok(products);
        }
    }
}
