using Azure;
using ShoppingCartAPI.Dto;
using ShoppingCartAPI.Exceptions;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Repositories;

namespace ShoppingCartAPI.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerService(CustomerRepository repo)
        {
            _customerRepository = repo;
        }

        // 1. Register
        public async Task RegisterAsync(RegisterDto dto)
        {
            Customer? customer = await _customerRepository.GetCustomerByNameAsync(dto.Name);
            if (customer != null)
                throw new CustomerAlreadyExistsException(dto.Name);

            await _customerRepository.AddCustomerAsync(dto.Name, dto.Password);
        }


        // 8. UpdateBudget
        public async Task UpdateBudgetAsync(int customerId, UpdateBudgetDto dto)
        {
            if (dto.Budget < 0)
                throw new InvalidBudgetValueException(dto.Budget);

            Customer? customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new CustomerNotFoundException(customerId);

            await _customerRepository.UpdateBudgetAsync(dto.Budget, customer);
        }
    }
}
