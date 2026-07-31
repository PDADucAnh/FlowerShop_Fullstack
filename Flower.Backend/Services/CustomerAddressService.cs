using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Flower.Backend.Services
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly IApplicationDbContext _context;

        public CustomerAddressService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerAddressDTO>> GetByCustomerId(int customerId)
        {
            var list = await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsActive)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.Id)
                .ToListAsync();
            return list.Select(ToDTO);
        }

        public async Task<CustomerAddressDTO?> GetById(int id)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            return address == null ? null : ToDTO(address);
        }

        public async Task<CustomerAddressDTO> Create(CreateCustomerAddressDTO dto)
        {
            if (dto.IsDefault)
            {
                var others = _context.CustomerAddresses
                    .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault);
                foreach (var a in others) a.IsDefault = false;
            }

            var address = new CustomerAddress
            {
                CustomerId = dto.CustomerId,
                ReceiverName = dto.ReceiverName,
                ReceiverPhone = dto.ReceiverPhone,
                Province = dto.Province,
                District = dto.District,
                Ward = dto.Ward,
                AddressLine = dto.AddressLine,
                PostalCode = dto.PostalCode,
                Note = dto.Note,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsDefault = dto.IsDefault
            };

            if (!await _context.CustomerAddresses.AnyAsync(a => a.CustomerId == dto.CustomerId))
            {
                address.IsDefault = true;
            }

            _context.CustomerAddresses.Add(address);
            await _context.SaveChangesAsync();
            return ToDTO(address);
        }

        public async Task<bool> Update(int id, UpdateCustomerAddressDTO dto)
        {
            if (id != dto.Id) return false;

            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            if (dto.IsDefault)
            {
                var others = _context.CustomerAddresses
                    .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault && a.Id != id);
                foreach (var a in others) a.IsDefault = false;
            }

            address.CustomerId = dto.CustomerId;
            address.ReceiverName = dto.ReceiverName;
            address.ReceiverPhone = dto.ReceiverPhone;
            address.Province = dto.Province;
            address.District = dto.District;
            address.Ward = dto.Ward;
            address.AddressLine = dto.AddressLine;
            address.PostalCode = dto.PostalCode;
            address.Note = dto.Note;
            address.Latitude = dto.Latitude;
            address.Longitude = dto.Longitude;
            address.IsDefault = dto.IsDefault;
            address.UpdatedAt = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null) return false;

            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefault(int id, int customerId)
        {
            var address = await _context.CustomerAddresses.FindAsync(id);
            if (address == null || address.CustomerId != customerId) return false;

            var others = _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId && a.IsDefault && a.Id != id);
            foreach (var a in others) a.IsDefault = false;

            address.IsDefault = true;
            address.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        private static CustomerAddressDTO ToDTO(CustomerAddress a)
        {
            return new CustomerAddressDTO
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                ReceiverName = a.ReceiverName,
                ReceiverPhone = a.ReceiverPhone,
                Province = a.Province,
                District = a.District,
                Ward = a.Ward,
                AddressLine = a.AddressLine,
                PostalCode = a.PostalCode,
                Note = a.Note,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                IsDefault = a.IsDefault,
                IsActive = a.IsActive
            };
        }
    }
}
