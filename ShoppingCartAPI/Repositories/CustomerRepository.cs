using System;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Context;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartAPI.Repositories
{
    public class CustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1.1 AddCustomer
        public async Task<Customer> AddCustomerAsync(string name, string password)
        {
            var customer = new Customer
            {
                Name = name,
                Password = password,
                Budget = 0
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        // 1.2 UpdateBudget
        public async Task UpdateBudgetAsync(decimal budget, Customer customer)
        {
            customer.Budget = budget;
            await _context.SaveChangesAsync();
        }

        // 1.3 GetCustomerByName
        public async Task<Customer?> GetCustomerByNameAsync(string name)
        {
            return await _context.Customers
                .Where(cu => cu.Name.Equals(name))
                .FirstOrDefaultAsync();
        }

        // 1.4 GetCustomerById
        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<Customer?> GetCustomerByCredentials(string name, string password)
        {
            return await _context.Customers
                .Where(cu => cu.Name.Equals(name))
                .Where(cu => cu.Password.Equals(password))
                .FirstOrDefaultAsync();
        }
    }

}
