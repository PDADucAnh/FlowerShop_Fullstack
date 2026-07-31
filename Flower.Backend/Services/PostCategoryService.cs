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
    public class PostCategoryService : IPostCategoryService
    {
        private readonly IApplicationDbContext _context;

        public PostCategoryService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostCategoryDTO>> GetAll()
        {
            var categories = await _context.PostCategories.Include(c => c.Posts).ToListAsync();
            return categories.Select(c => c.ToDTO());
        }

        public async Task<PagedResult<PostCategoryDTO>> GetPaged(int page, int pageSize)
        {
            var query = _context.PostCategories.OrderByDescending(c => c.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Include(c => c.Posts)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PostCategoryDTO>
            {
                Items = items.Select(c => c.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PostCategoryDTO?> GetById(int id)
        {
            var category = await _context.PostCategories
                .Include(c => c.Posts)
                .FirstOrDefaultAsync(c => c.Id == id);
            return category?.ToDTO();
        }

        public async Task<PostCategoryDTO> Create(CreatePostCategoryDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Slug))
            {
                dto.Slug = Flower.Backend.Utils.SlugHelper.GenerateSlug(dto.Name);
            }
            var category = dto.ToEntity();
            _context.PostCategories.Add(category);
            await _context.SaveChangesAsync();
            return category.ToDTO();
        }

        public async Task<bool> Update(int id, UpdatePostCategoryDTO dto)
        {
            if (id != dto.Id)
                return false;

            var category = await _context.PostCategories.FindAsync(id);
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
                if (!await _context.PostCategories.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var category = await _context.PostCategories.FindAsync(id);
            if (category == null)
                return false;

            _context.PostCategories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
