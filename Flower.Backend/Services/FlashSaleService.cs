using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
            var now = DateTimeUtils.GetVietnamTime();
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

        public async Task<int> BulkAdd(BulkAddFlashSaleProductsDto dto)
        {
            if (dto.Products == null || dto.Products.Count == 0)
                throw new InvalidOperationException("Danh sách sản phẩm không được để trống.");

            var flashSaleExists = await _context.FlashSales.AnyAsync(fs => fs.Id == dto.FlashSaleId);
            if (!flashSaleExists)
                throw new KeyNotFoundException($"Không tìm thấy Flash Sale với id {dto.FlashSaleId}.");

            var productIds = dto.Products.Select(p => p.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in dto.Products)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new InvalidOperationException($"Không tìm thấy sản phẩm với id {item.ProductId}.");

                if (item.SalePrice > product.Price)
                    throw new InvalidOperationException($"Giá Flash Sale của sản phẩm '{product.Name}' không được lớn hơn giá gốc.");

                if (item.Quantity < 0)
                    throw new InvalidOperationException($"Số lượng của sản phẩm '{product.Name}' không được âm.");

                var existing = await _context.FlashSaleProducts
                    .FirstOrDefaultAsync(fsp => fsp.FlashSaleId == dto.FlashSaleId && fsp.ProductId == item.ProductId);

                var discountPercent = product.Price > 0
                    ? Math.Round((product.Price - item.SalePrice) / product.Price * 100, 2)
                    : 0;

                if (existing != null)
                {
                    existing.SalePrice = item.SalePrice;
                    existing.Quantity = item.Quantity;
                    existing.DiscountPercent = discountPercent;
                }
                else
                {
                    _context.FlashSaleProducts.Add(new FlashSaleProduct
                    {
                        FlashSaleId = dto.FlashSaleId,
                        ProductId = item.ProductId,
                        SalePrice = item.SalePrice,
                        Quantity = item.Quantity,
                        DiscountPercent = discountPercent,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return dto.Products.Count;
        }

        public async Task<IEnumerable<FlashSaleProductPreviewDto>> PreviewByCategory(FlashSalePreviewRequestDto dto)
        {
            if (!await FlashSaleExists(dto.FlashSaleId))
                throw new KeyNotFoundException($"Không tìm thấy Flash Sale với id {dto.FlashSaleId}.");

            var existingIds = await GetExistingProductIds(dto.FlashSaleId);
            var discountPercent = NormalizeDiscountPercent(dto.DefaultDiscountPercent);

            var query = _context.Products.Where(p => p.IsActive && !existingIds.Contains(p.Id));

            if (dto.ProductCategoryIds != null && dto.ProductCategoryIds.Count > 0)
                query = query.Where(p => dto.ProductCategoryIds.Contains(p.ProductCategoryId));

            var products = await query
                .OrderBy(p => p.Name)
                .ToListAsync();

            return products.Select(p => BuildPreview(p, discountPercent));
        }

        public async Task<IEnumerable<FlashSaleProductPreviewDto>> PreviewByBestSeller(FlashSalePreviewRequestDto dto)
        {
            if (!await FlashSaleExists(dto.FlashSaleId))
                throw new KeyNotFoundException($"Không tìm thấy Flash Sale với id {dto.FlashSaleId}.");

            var existingIds = await GetExistingProductIds(dto.FlashSaleId);
            var discountPercent = NormalizeDiscountPercent(dto.DefaultDiscountPercent);
            var topCount = dto.TopCount is > 0 ? dto.TopCount.Value : 10;

            var since = DateTimeUtils.GetVietnamTime().AddDays(-30);

            var bestSellers = await _context.OrderDetails
                .Where(od => od.Order != null
                    && od.Order.Status == OrderStatus.Completed
                    && od.Order.OrderDate >= since)
                .GroupBy(od => od.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(od => od.Quantity) })
                .OrderByDescending(x => x.TotalSold)
                .Take(topCount)
                .ToListAsync();

            var bestSellerIds = bestSellers.Select(b => b.ProductId).ToList();
            var products = await _context.Products
                .Where(p => p.IsActive
                    && !existingIds.Contains(p.Id)
                    && bestSellerIds.Contains(p.Id)
                    && (!dto.MinStockQuantity.HasValue || p.StockQuantity >= dto.MinStockQuantity.Value))
                .ToListAsync();

            return products.Select(p => BuildPreview(p, discountPercent));
        }

        public async Task<IEnumerable<FlashSaleProductPreviewDto>> PreviewByExcel(int flashSaleId, decimal? defaultDiscountPercent, IFormFile file)
        {
            if (!await FlashSaleExists(flashSaleId))
                throw new KeyNotFoundException($"Không tìm thấy Flash Sale với id {flashSaleId}.");

            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Vui lòng chọn file Excel.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".xlsx")
                throw new InvalidOperationException("File Excel phải có định dạng .xlsx");

            var existingIds = await GetExistingProductIds(flashSaleId);

            using var excelStream = new MemoryStream();
            await file.CopyToAsync(excelStream);
            excelStream.Position = 0;

            List<(string Sku, decimal? SalePrice, int Quantity)> rows;
            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet?.Dimension == null)
                    throw new InvalidOperationException("File Excel không có dữ liệu.");

                rows = new List<(string, decimal?, int)>();
                var rowCount = worksheet.Dimension.Rows;
                for (int row = 2; row <= rowCount; row++)
                {
                    var sku = worksheet.GetValue<string>(row, 1)?.Trim();
                    var priceText = worksheet.GetValue<string>(row, 2)?.Trim();
                    var quantityText = worksheet.GetValue<string>(row, 3)?.Trim();

                    if (string.IsNullOrWhiteSpace(sku)) continue;

                    decimal? salePrice = null;
                    if (decimal.TryParse(priceText, out var price))
                        salePrice = price;

                    int quantity = 0;
                    int.TryParse(quantityText, out quantity);

                    rows.Add((sku, salePrice, quantity));
                }
            }

            var skus = rows.Select(r => r.Sku).Distinct().ToList();
            var products = await _context.Products
                .Where(p => p.Sku != null && skus.Contains(p.Sku))
                .ToDictionaryAsync(p => p.Sku!, StringComparer.OrdinalIgnoreCase);

            var result = new List<FlashSaleProductPreviewDto>();
            foreach (var row in rows)
            {
                if (!products.TryGetValue(row.Sku, out var product)) continue;
                if (existingIds.Contains(product.Id)) continue;

                var discountPercent = NormalizeDiscountPercent(defaultDiscountPercent);
                var suggestedPrice = row.SalePrice ??
                    (product.Price > 0 ? Math.Round(product.Price * (1 - discountPercent / 100), 2) : 0);

                var preview = BuildPreview(product, discountPercent);
                preview.SuggestedSalePrice = suggestedPrice;
                preview.DiscountPercent = product.Price > 0
                    ? Math.Round((product.Price - suggestedPrice) / product.Price * 100, 2)
                    : 0;
                if (row.Quantity > 0) preview.Quantity = row.Quantity;

                result.Add(preview);
            }

            return result;
        }

        private async Task<bool> FlashSaleExists(int flashSaleId)
        {
            return await _context.FlashSales.AnyAsync(fs => fs.Id == flashSaleId);
        }

        private async Task<HashSet<int>> GetExistingProductIds(int flashSaleId)
        {
            return (await _context.FlashSaleProducts
                .Where(fsp => fsp.FlashSaleId == flashSaleId)
                .Select(fsp => fsp.ProductId)
                .ToListAsync()).ToHashSet();
        }

        private static decimal NormalizeDiscountPercent(decimal? discountPercent)
        {
            var value = discountPercent ?? 15;
            return Math.Clamp(value, 0, 100);
        }

        private static FlashSaleProductPreviewDto BuildPreview(Product product, decimal discountPercent)
        {
            var suggestedPrice = product.Price > 0
                ? Math.Round(product.Price * (1 - discountPercent / 100), 2)
                : 0;

            return new FlashSaleProductPreviewDto
            {
                ProductId = product.Id,
                Sku = product.Sku,
                ProductName = product.Name,
                ProductImageUrl = product.ImageUrl,
                OriginalPrice = product.Price,
                StockQuantity = product.StockQuantity,
                SuggestedSalePrice = suggestedPrice,
                Quantity = product.StockQuantity,
                DiscountPercent = product.Price > 0
                    ? Math.Round((product.Price - suggestedPrice) / product.Price * 100, 2)
                    : 0
            };
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
