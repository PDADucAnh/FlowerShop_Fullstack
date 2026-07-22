using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class PageService : IPageService
    {
        private readonly IApplicationDbContext _context;

        public PageService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PageDTO>> GetAll()
        {
            var items = await _context.Pages
                .OrderBy(p => p.Title)
                .ToListAsync();
            return items.Select(p => p.ToDTO());
        }

        public async Task<IEnumerable<PageDTO>> GetAllActive()
        {
            var items = await _context.Pages
                .Where(p => p.IsActive)
                .OrderBy(p => p.Title)
                .ToListAsync();
            return items.Select(p => p.ToDTO());
        }

        public async Task<PageDTO?> GetById(int id)
        {
            var entity = await _context.Pages.FindAsync(id);
            return entity?.ToDTO();
        }

        public async Task<PageDTO?> GetBySlug(string slug)
        {
            var entity = await _context.Pages
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);
            return entity?.ToDTO();
        }

        public async Task<PageDTO> Create(CreatePageDTO dto)
        {
            var entity = dto.ToEntity();
            _context.Pages.Add(entity);
            await _context.SaveChangesAsync();
            return entity.ToDTO();
        }

        public async Task<bool> Update(int id, UpdatePageDTO dto)
        {
            if (id != dto.Id) return false;
            var entity = await _context.Pages.FindAsync(id);
            if (entity == null) return false;
            dto.UpdateEntity(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Pages.FindAsync(id);
            if (entity == null) return false;
            _context.Pages.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
