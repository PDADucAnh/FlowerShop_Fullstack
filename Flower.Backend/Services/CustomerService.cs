using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IApplicationDbContext _context;
        private readonly PasswordHasher<Customer> _passwordHasher;

        public CustomerService(IApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Customer>();
        }

        public async Task<IEnumerable<CustomerDTO>> GetAll()
        {
            var list = await _context.Customers.ToListAsync();
            return list.Select(c => c.ToDTO());
        }

        public async Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize)
        {
            var query = _context.Customers.OrderByDescending(c => c.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CustomerDTO>
            {
                Items = items.Select(c => c.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDTO?> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer?.ToDTO();
        }

        public async Task<CustomerDTO> Create(CreateCustomerDTO dto)
        {
            var customer = dto.ToEntity();
            customer.PasswordHash = _passwordHasher.HashPassword(customer, customer.PasswordHash);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateCustomerDTO dto)
        {
            if (id != dto.Id)
                return false;

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            dto.UpdateEntity(customer);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Customers.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
