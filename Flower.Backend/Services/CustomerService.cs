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
        private readonly ICustomerNotificationService _notificationService;

        public CustomerService(IApplicationDbContext context, ICustomerNotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
            _passwordHasher = new PasswordHasher<Customer>();
        }

        public async Task<IEnumerable<CustomerDTO>> GetAll()
        {
            var list = await _context.Customers.ToListAsync();
            return list.Select(c => c.ToDTO());
        }

        public async Task<PagedResult<CustomerDTO>> GetPaged(int page, int pageSize, string? search = null)
        {
            IQueryable<Customer> query = _context.Customers.OrderByDescending(c => c.CreatedAt);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.FullName.Contains(search) ||
                    c.Email.Contains(search) ||
                    (c.Phone != null && c.Phone.Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items.Select(c => new CustomerDTO
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                TotalOrders = c.TotalOrders,
                SuccessfulDeliveries = c.SuccessfulDeliveries,
                FailedDeliveries = c.FailedDeliveries,
                IsBlacklisted = c.IsBlacklisted,
                FraudScore = c.FraudScore,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();

            return new PagedResult<CustomerDTO>
            {
                Items = dtos,
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

            var wasActive = customer.IsActive;
            dto.UpdateEntity(customer);

            try
            {
                await _context.SaveChangesAsync();

                if (wasActive && !customer.IsActive)
                {
                    await _notificationService.NotifyCustomerEvent(customer.Id, "CustomerLocked", new { reason = "Admin locked account" });
                }

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
