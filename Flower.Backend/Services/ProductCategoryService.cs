using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPhotoService _photoService;

        public ProductCategoryService(IApplicationDbContext context, IPhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        public async Task<IEnumerable<ProductCategoryDTO>> GetAll()
        {
            var list = await _context.ProductCategories.ToListAsync();
            return list.Select(c => c.ToDTO());
        }

        public async Task<PagedResult<ProductCategoryDTO>> GetPaged(int page, int pageSize)
        {
            var query = _context.ProductCategories.OrderByDescending(cp => cp.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductCategoryDTO>
            {
                Items = items.Select(cp => cp.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProductCategoryDTO?> GetById(int id)
        {
            var category = await _context.ProductCategories
                .FirstOrDefaultAsync(cp => cp.Id == id);
            return category?.ToDTO();
        }

        public async Task<ProductCategoryDTO> Create(CreateProductCategoryDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Slug))
            {
                dto.Slug = Flower.Backend.Utils.SlugHelper.GenerateSlug(dto.Name);
            }
            var category = dto.ToEntity();
            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();
            return category.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateProductCategoryDTO dto)
        {
            if (id != dto.Id)
                return false;

            var category = await _context.ProductCategories.FindAsync(id);
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
                if (!await _context.ProductCategories.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);
            if (category == null)
                return false;

            if (!string.IsNullOrEmpty(category.ImageUrl))
                await _photoService.DeletePhotoAsync(category.ImageUrl);

            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
