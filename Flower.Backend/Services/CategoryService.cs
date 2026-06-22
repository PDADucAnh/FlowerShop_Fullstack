using Flower.Data;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IApplicationDbContext _context;

        public CategoryService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            var categories = await _context.Categories.ToListAsync();
            return categories.Select(c => c.ToDTO());
        }

        public async Task<CategoryDTO?> GetById(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.Id == id);
            return category?.ToDTO();
        }

        public async Task<CategoryDTO> Create(CreateCategoryDTO dto)
        {
            var category = dto.ToEntity();
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateCategoryDTO dto)
        {
            if (id != dto.Id)
                return false;

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            dto.UpdateEntity(category);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categories.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
