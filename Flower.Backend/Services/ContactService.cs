using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flower.Backend.Models;

namespace Flower.Backend.Services
{
    public class ContactService : IContactService
    {
        private readonly IApplicationDbContext _context;

        public ContactService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactDTO>> GetAll()
        {
            var items = await _context.Contacts
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return items.Select(c => c.ToDTO());
        }

        public async Task<PagedResult<ContactDTO>> GetPaged(int page, int pageSize, bool? isRead = null)
        {
            IQueryable<Contact> query = _context.Contacts.OrderByDescending(c => c.CreatedAt);

            if (isRead.HasValue)
                query = query.Where(c => c.IsRead == isRead.Value);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = items.Select(c => new ContactDTO
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Subject = c.Subject,
                Message = c.Message,
                IsRead = c.IsRead,
                ReadAt = c.ReadAt,
                CreatedAt = c.CreatedAt
            }).ToList();

            return new PagedResult<ContactDTO>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ContactDTO?> GetById(int id)
        {
            var entity = await _context.Contacts.FindAsync(id);
            return entity?.ToDTO();
        }

        public async Task<ContactDTO> Create(CreateContactDTO dto)
        {
            var entity = dto.ToEntity();
            _context.Contacts.Add(entity);
            await _context.SaveChangesAsync();
            return entity.ToDTO();
        }

        public async Task<bool> MarkRead(int id, bool isRead)
        {
            var entity = await _context.Contacts.FindAsync(id);
            if (entity == null) return false;
            entity.IsRead = isRead;
            entity.ReadAt = isRead ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Contacts.FindAsync(id);
            if (entity == null) return false;
            _context.Contacts.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadCount()
        {
            return await _context.Contacts.CountAsync(c => !c.IsRead);
        }
    }
}
