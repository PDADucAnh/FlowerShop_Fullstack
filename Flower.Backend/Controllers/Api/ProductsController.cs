using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Flower.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Flower.Data;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "StaffOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly IApplicationDbContext _context;
        private readonly IPriceCalculationService _priceCalculationService;

        public ProductsController(
            IProductService productService, 
            INotificationService notificationService,
            IApplicationDbContext context,
            IPriceCalculationService priceCalculationService)
        {
            _productService = productService;
            _notificationService = notificationService;
            _context = context;
            _priceCalculationService = priceCalculationService;
        }

        [AllowAnonymous]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 8,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? categoryProductId = null)
        {
            var result = await _productService.GetPaged(page, pageSize, minPrice, maxPrice, categoryProductId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var results = await _productService.Search(query);
            return Ok(results);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAll();
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("categoryproduct/{categoryProductId}")]
        public async Task<IActionResult> GetByCategoryProduct(int categoryProductId)
        {
            var products = await _productService.GetByCategoryProduct(categoryProductId);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending([FromQuery] int count = 10)
        {
            var products = await _productService.GetTrending(count);
            return Ok(products);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var product = await _productService.GetDetail(id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm này trong hệ thống" });
            }

            await _productService.TrackView(id);

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _productService.Create(dto);
            await _notificationService.NotifyEntityChanged("Product");
            return CreatedAtAction(nameof(GetDetail), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _productService.Update(id, dto);

            if (!updated)
                return NotFound();

            await _notificationService.NotifyEntityChanged("Product");
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("{id}/track-add-to-cart")]
        public async Task<IActionResult> TrackAddToCart(int id)
        {
            await _productService.TrackAddToCart(id);
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.Delete(id);

            if (!deleted)
                return NotFound();

            await _notificationService.NotifyEntityChanged("Product");
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequest request)
        {
            if (request?.ProductIds == null || request.ProductIds.Count == 0)
                return BadRequest(new { message = "Danh sách sản phẩm không hợp lệ" });

            var deletedCount = await _productService.BulkDeleteAsync(request.ProductIds);
            await _notificationService.NotifyEntityChanged("Product");

            return Ok(new
            {
                deletedCount,
                message = $"Đã xóa {deletedCount} sản phẩm"
            });
        }

        [AllowAnonymous]
        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetImages(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product.Images.Select(i => i.ToDTO()).ToList());
        }

        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddImage(int id, [FromBody] AddProductImageRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            var maxSort = await _context.ProductImages
                .Where(i => i.ProductId == id)
                .MaxAsync(i => (int?)i.SortOrder) ?? -1;

            var image = new ProductImage
            {
                ProductId = id,
                ImageUrl = request.ImageUrl,
                SortOrder = maxSort + 1
            };

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDetail), new { id }, image.ToDTO());
        }

        [HttpDelete("{id}/images/{imageId}")]
        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            var image = await _context.ProductImages
                .FirstOrDefaultAsync(i => i.Id == imageId && i.ProductId == id);

            if (image == null)
                return NotFound();

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("recalculate-cart")]
        public async Task<IActionResult> RecalculateCart([FromBody] CartRecalculateRequest request)
        {
            if (request == null || request.Items == null)
                return BadRequest(new { message = "Dữ liệu giỏ hàng không hợp lệ" });

            var response = new CartRecalculateResponse();
            bool priceChanged = false;

            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();
            var productDict = products.ToDictionary(p => p.Id);

            // Get bulk calculated prices (with promotions applied at Vietnam Standard time)
            var priceCalculations = await _priceCalculationService.CalculateBulkPrices(productIds);

            foreach (var item in request.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                {
                    continue;
                }

                // Determine size adjustment based on name
                decimal sizeAdjustment = 0;
                if (item.Name.EndsWith("(Deluxe)")) sizeAdjustment = 300000;
                else if (item.Name.EndsWith("(Grand)")) sizeAdjustment = 600000;

                // Load price calculation details
                priceCalculations.TryGetValue(item.ProductId, out var calc);
                
                var baseOriginalPrice = calc?.OriginalPrice ?? product.Price;
                var latestOriginalPrice = baseOriginalPrice + sizeAdjustment;
                
                decimal? latestPromotionPrice = null;
                if (calc != null && calc.PromotionPrice.HasValue)
                {
                    latestPromotionPrice = calc.PromotionPrice.Value + sizeAdjustment;
                }

                var latestCurrentPrice = latestPromotionPrice ?? latestOriginalPrice;
                var hasFlashSale = calc?.HasFlashSale ?? false;
                var promotionPercent = calc?.PromotionPercent;
                var promotionName = calc?.PromotionName;

                // Check if price changed compared to what was sent
                var clientOriginalPrice = item.Price;
                var clientPromotionPrice = item.PromotionPrice;

                if (latestOriginalPrice != clientOriginalPrice || latestPromotionPrice != clientPromotionPrice)
                {
                    priceChanged = true;
                }

                response.Items.Add(new CartItemRecalculatedDTO
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = latestOriginalPrice,
                    PromotionPrice = latestPromotionPrice,
                    CurrentPrice = latestCurrentPrice,
                    HasFlashSale = hasFlashSale,
                    PromotionPercent = promotionPercent,
                    PromotionName = promotionName,
                    ImageUrl = product.ImageUrl,
                    StockQuantity = product.StockQuantity,
                    Description = product.Description
                });
            }

            response.PriceChanged = priceChanged;
            if (priceChanged)
            {
                response.Message = "Giá của một hoặc nhiều sản phẩm đã được cập nhật do chương trình khuyến mãi đã kết thúc hoặc thay đổi.";
            }

            return Ok(response);
        }
    }
}
