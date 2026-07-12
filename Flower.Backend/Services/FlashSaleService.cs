using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class FlashSaleService : IFlashSaleService
    {
        private readonly IApplicationDbContext _context;

        public FlashSaleService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FlashSaleActiveDTO>> GetActiveFlashSales()
        {
            var now = DateTime.UtcNow;
            var activeFlashSales = await _context.FlashSales
                .Include(fs => fs.FlashSaleProducts)
                    .ThenInclude(fsp => fsp.Product)
                .Where(fs => fs.IsActive && fs.StartDate <= now && fs.EndDate > now)
                .ToListAsync();

            var result = new List<FlashSaleActiveDTO>();
            foreach (var fs in activeFlashSales)
            {
                if (fs.FlashSaleProducts == null) continue;
                foreach (var fp in fs.FlashSaleProducts)
                {
                    if (fp.Product == null) continue;
                    var originalPrice = fp.Product.Price;
                    var discountPercent = originalPrice > 0
                        ? Math.Round((originalPrice - fp.SalePrice) / originalPrice * 100, 2)
                        : 0;
                    result.Add(new FlashSaleActiveDTO
                    {
                        ProductId = fp.ProductId,
                        ProductName = fp.Product.Name,
                        ProductImageUrl = fp.Product.ImageUrl,
                        OriginalPrice = originalPrice,
                        SalePrice = fp.SalePrice,
                        DiscountPercent = discountPercent,
                        PromotionName = fs.Name,
                        PromotionEndTime = fs.EndDate
                    });
                }
            }
            return result;
        }

        public async Task<IEnumerable<FlashSaleDTO>> GetAll()
        {
            var items = await _context.FlashSales
                .Include(fs => fs.FlashSaleProducts)
                    .ThenInclude(fsp => fsp.Product)
                .OrderByDescending(fs => fs.CreatedAt)
                .ToListAsync();

            return items.Select(fs => MapToDTO(fs));
        }

        public async Task<FlashSaleDTO?> GetById(int id)
        {
            var item = await _context.FlashSales
                .Include(fs => fs.FlashSaleProducts)
                    .ThenInclude(fsp => fsp.Product)
                .FirstOrDefaultAsync(fs => fs.Id == id);
            return item == null ? null : MapToDTO(item);
        }

        public async Task<FlashSaleDTO> Create(CreateFlashSaleDTO dto)
        {
            var entity = new FlashSale
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.Products != null)
            {
                entity.FlashSaleProducts = dto.Products.Select(p => new FlashSaleProduct
                {
                    ProductId = p.ProductId,
                    SalePrice = p.SalePrice
                }).ToList();
            }

            _context.FlashSales.Add(entity);
            await _context.SaveChangesAsync();

            return (await GetById(entity.Id))!;
        }

        public async Task<bool> Update(int id, UpdateFlashSaleDTO dto)
        {
            var entity = await _context.FlashSales
                .Include(fs => fs.FlashSaleProducts)
                .FirstOrDefaultAsync(fs => fs.Id == id);
            if (entity == null) return false;

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.StartDate.HasValue) entity.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) entity.EndDate = dto.EndDate.Value;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;
            entity.UpdatedAt = DateTime.UtcNow;

            if (dto.Products != null)
            {
                if (entity.FlashSaleProducts != null)
                    _context.FlashSaleProducts.RemoveRange(entity.FlashSaleProducts);
                entity.FlashSaleProducts = dto.Products.Select(p => new FlashSaleProduct
                {
                    FlashSaleId = id,
                    ProductId = p.ProductId,
                    SalePrice = p.SalePrice
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.FlashSales
                .Include(fs => fs.FlashSaleProducts)
                .FirstOrDefaultAsync(fs => fs.Id == id);
            if (entity == null) return false;

            if (entity.FlashSaleProducts != null)
                _context.FlashSaleProducts.RemoveRange(entity.FlashSaleProducts);
            _context.FlashSales.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private FlashSaleDTO MapToDTO(FlashSale fs)
        {
            return new FlashSaleDTO
            {
                Id = fs.Id,
                Name = fs.Name,
                Description = fs.Description,
                StartDate = fs.StartDate,
                EndDate = fs.EndDate,
                IsActive = fs.IsActive,
                CreatedAt = fs.CreatedAt,
                UpdatedAt = fs.UpdatedAt,
                Products = fs.FlashSaleProducts?.Select(fp => new FlashSaleProductDTO
                {
                    Id = fp.Id,
                    FlashSaleId = fp.FlashSaleId,
                    ProductId = fp.ProductId,
                    ProductName = fp.Product?.Name,
                    ProductImageUrl = fp.Product?.ImageUrl,
                    OriginalPrice = fp.Product?.Price ?? 0,
                    SalePrice = fp.SalePrice,
                    DiscountPercent = fp.Product != null && fp.Product.Price > 0
                        ? Math.Round((fp.Product.Price - fp.SalePrice) / fp.Product.Price * 100, 2)
                        : 0
                }).ToList()
            };
        }
    }
}
