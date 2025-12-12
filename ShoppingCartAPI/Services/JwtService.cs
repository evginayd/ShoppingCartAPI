using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repositories;

namespace ShoppingCartAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly CustomerRepository _customerRepository;

        public JwtService(IConfiguration config, CustomerRepository repo)
        {
            _config = config;
            _customerRepository = repo;

        }

        public string GenerateToken(int userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            Customer? customer = await _customerRepository.GetCustomerByCredentials(dto.Name, dto.Password);
            if (customer == null)
                throw new InvalidCredentialsException();
            return this.GenerateToken(customer.Id, customer.Name);
        }
    }
}
