using Flower.Data;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class CategoryProductService : ICategoryProductService
    {
        private readonly IApplicationDbContext _context;

        public CategoryProductService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryProductDTO>> GetAll()
        {
            var list = await _context.CategoriesProducts.ToListAsync();
            return list.Select(c => c.ToDTO());
        }

        public async Task<CategoryProductDTO?> GetById(int id)
        {
            var category = await _context.CategoriesProducts.FindAsync(id);
            return category?.ToDTO();
        }

        public async Task<CategoryProductDTO> Create(CreateCategoryProductDTO dto)
        {
            var category = dto.ToEntity();
            _context.CategoriesProducts.Add(category);
            await _context.SaveChangesAsync();
            return category.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateCategoryProductDTO dto)
        {
            if (id != dto.Id)
                return false;

            var category = await _context.CategoriesProducts.FindAsync(id);
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
                if (!await _context.CategoriesProducts.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.CategoriesProducts.FindAsync(id);
            if (category == null)
                return false;

            _context.CategoriesProducts.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
