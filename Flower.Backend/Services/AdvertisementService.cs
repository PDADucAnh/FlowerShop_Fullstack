using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flower.Backend.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IApplicationDbContext _context;

        public AdvertisementService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetAllActive()
        {
            var items = await _context.Advertisements
                .Where(a => a.IsActive)
                .OrderBy(a => a.SortOrder)
                .ThenByDescending(a => a.CreatedDate)
                .ToListAsync();

            return items.Select(a => a.ToDTO());
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetAll()
        {
            var items = await _context.Advertisements
                .OrderBy(a => a.SortOrder)
                .ThenByDescending(a => a.CreatedDate)
                .ToListAsync();

            return items.Select(a => a.ToDTO());
        }

        public async Task<AdvertisementDTO?> GetById(int id)
        {
            var item = await _context.Advertisements.FindAsync(id);
            return item?.ToDTO();
        }

        public async Task<AdvertisementDTO> Create(CreateAdvertisementDTO dto)
        {
            var entity = new Advertisement
            {
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                ImageUrl = dto.ImageUrl,
                LinkUrl = dto.LinkUrl,
                SortOrder = dto.SortOrder,
                IsActive = dto.IsActive,
                CreatedDate = System.DateTime.UtcNow
            };

            _context.Advertisements.Add(entity);
            await _context.SaveChangesAsync();

            return entity.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateAdvertisementDTO dto)
        {
            if (id != dto.Id)
                return false;

            var entity = await _context.Advertisements.FindAsync(id);
            if (entity == null)
                return false;

            entity.Title = dto.Title;
            entity.Subtitle = dto.Subtitle;
            entity.ImageUrl = dto.ImageUrl;
            entity.LinkUrl = dto.LinkUrl;
            entity.SortOrder = dto.SortOrder;
            entity.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Advertisements.FindAsync(id);
            if (entity == null)
                return false;

            _context.Advertisements.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
