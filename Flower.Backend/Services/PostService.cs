using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Flower.Backend.Services
{
    public class PostService : IPostService
    {
        private readonly IApplicationDbContext _context;

        public PostService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostDTO>> GetAll()
        {
            var list = await _context.Posts
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return list.Select(p => p.ToDTO());
        }

        public async Task<PagedResult<PostDTO>> GetPaged(int page, int pageSize)
        {
            var query = _context.Posts
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PostDTO>
            {
                Items = items.Select(p => p.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PostDTO?> GetById(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            return post?.ToDTO();
        }

        public async Task<IEnumerable<PostDTO>> GetByCategory(int categoryId)
        {
            var posts = await _context.Posts
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return posts.Select(p => p.ToDTO());
        }

        public async Task<PostDTO> Create(CreatePostDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Slug))
            {
                dto.Slug = Flower.Backend.Utils.SlugHelper.GenerateSlug(dto.Title);
            }
            var post = dto.ToEntity();
            post.CreatedDate = DateTime.UtcNow;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            await _context.Entry(post)
                .Reference(p => p.Category)
                .LoadAsync();

            return post.ToDTO();
        }

        public async Task<bool> Update(int id, UpdatePostDTO dto)
        {
            if (id != dto.Id)
                return false;

            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
                return false;

            dto.UpdateEntity(post);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Posts.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return false;

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
