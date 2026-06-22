using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "StaffOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;

        public ProductsController(IProductService productService, INotificationService notificationService)
        {
            _productService = productService;
            _notificationService = notificationService;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var product = await _productService.GetDetail(id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm này trong hệ thống" });
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _productService.Create(dto);
            await _notificationService.NotifyEntityChanged("Product");
            return CreatedAtAction(nameof(GetDetail), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDTO dto)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.Delete(id);

            if (!deleted)
                return NotFound();

            await _notificationService.NotifyEntityChanged("Product");
            return NoContent();
        }
    }
}
