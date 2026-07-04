using System;
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
    public class ProductService : IProductService
    {
        private readonly IApplicationDbContext _context;

        public ProductService(IApplicationDbContext context)
        {
            _context = context;
        }


        private IQueryable<Product> BuildQuery()
        {
            return _context.Products
                .Include(p => p.CategoryProduct);
        }

        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            var products = await BuildQuery()
                .OrderByDescending(p => p.Id)
                .ToListAsync();
            return products.Select(p => p.ToDTO());
        }

        public async Task<PagedResult<ProductDTO>> GetPaged(int page, int pageSize, decimal? minPrice = null, decimal? maxPrice = null, int? categoryProductId = null)
        {
            var query = BuildQuery();

            if (categoryProductId.HasValue)
            {
                query = query.Where(p => p.CategoryProductId == categoryProductId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            query = query.OrderByDescending(p => p.Id);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDTO>
            {
                Items = items.Select(p => p.ToDTO()).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<ProductDTO>> GetByCategoryProduct(int categoryProductId)
        {
            var products = await BuildQuery()
                .Where(p => p.CategoryProductId == categoryProductId)
                .ToListAsync();
            return products.Select(p => p.ToDTO());
        }

        public async Task<ProductDTO?> GetDetail(int id)
        {
            var product = await BuildQuery()
                .FirstOrDefaultAsync(p => p.Id == id);
            return product?.ToDTO();
        }

        public async Task<ProductDTO> Create(CreateProductDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Slug))
            {
                dto.Slug = Flower.Backend.Utils.SlugHelper.GenerateSlug(dto.Name);
            }
            if (string.IsNullOrEmpty(dto.Sku))
            {
                dto.Sku = Flower.Backend.Utils.SlugHelper.GenerateSku(dto.Name);
            }

            var product = dto.ToEntity();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product)
                .Reference(p => p.CategoryProduct)
                .LoadAsync();

            return product.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateProductDTO dto)
        {
            if (id != dto.Id)
                return false;

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return false;

            if (string.IsNullOrEmpty(dto.ImageUrl))
            {
                dto.ImageUrl = product.ImageUrl;
            }

            dto.UpdateEntity(product);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Products.AnyAsync(e => e.Id == id))
                    return false;
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDTO>> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return new List<ProductDTO>();
            }

            var cleanQuery = query.Trim().ToLower();
            var products = await BuildQuery()
                .Where(p => p.Name.ToLower().Contains(cleanQuery) || 
                           (p.Sku != null && p.Sku.ToLower().Contains(cleanQuery)))
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return products.Select(p => p.ToDTO());
        }

        public async Task TrackView(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task TrackAddToCart(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.AddToCartCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductDTO>> GetTrending(int count = 10)
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);

            var sales7d = await _context.OrderDetails
                .Where(od => od.Order != null && od.Order.OrderDate >= sevenDaysAgo)
                .GroupBy(od => od.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQty = g.Sum(od => od.Quantity) })
                .ToListAsync();

            var sales30d = await _context.OrderDetails
                .Where(od => od.Order != null && od.Order.OrderDate >= thirtyDaysAgo && od.Order.OrderDate < sevenDaysAgo)
                .GroupBy(od => od.ProductId)
                .Select(g => new { ProductId = g.Key, TotalQty = g.Sum(od => od.Quantity) })
                .ToListAsync();

            var sales7dLookup = sales7d.ToDictionary(x => x.ProductId, x => x.TotalQty);
            var sales30dLookup = sales30d.ToDictionary(x => x.ProductId, x => x.TotalQty);

            const double W1 = 3.0;
            const double W2 = 1.0;
            const double W3 = 0.5;

            var products = await BuildQuery()
                .Where(p => p.StockQuantity > 0)
                .ToListAsync();

            var scored = products.Select(p =>
            {
                var s7d = sales7dLookup.GetValueOrDefault(p.Id, 0);
                var s30d = sales30dLookup.GetValueOrDefault(p.Id, 0);
                var score = (W1 * s7d) + (W2 * s30d) + (W3 * p.ViewCount);
                var dto = p.ToDTO();
                dto.TrendingScore = score;
                if (s7d >= 3)
                    dto.TrendingBadge = "Bán chạy";
                else if (score >= 10)
                    dto.TrendingBadge = "Hot";
                else if (score >= 5)
                    dto.TrendingBadge = "Trending";
                return dto;
            })
            .OrderByDescending(p => p.TrendingScore)
            .Take(count)
            .ToList();

            return scored;
        }
    }
}
