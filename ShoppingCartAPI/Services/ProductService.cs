using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repositories;

namespace ShoppingCartAPI.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;

        public ProductService(ProductRepository repo)
        {
            _productRepository = repo;
        }

        // 3. Home -> Tüm ürünleri getir
        public async Task<List<Product>> HomeAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        // 4. FilterProducts
        public async Task<List<Product>> FilterProductsAsync(FilterProductDto dto)
        {
            // IQueryable üzerinden başlanmalı
            IQueryable<Product> query = _productRepository.GetQueryable();

            if (dto.PriceMin.HasValue)
                query = query.Where(p => p.Price >= dto.PriceMin.Value);

            if (dto.PriceMax.HasValue)
                query = query.Where(p => p.Price <= dto.PriceMax.Value);

            if (!string.IsNullOrEmpty(dto.Category))
                query = query.Where(p => p.Category == dto.Category);

            if (!string.IsNullOrEmpty(dto.Name))
                query = query.Where(p => p.Name.Contains(dto.Name));

            // Tüm filtreler uygulandıktan sonra tek seferde liste al
            return await query.ToListAsync();
        }

    }

}
