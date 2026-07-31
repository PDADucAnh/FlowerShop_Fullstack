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
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryService _categoryProductService;
        private readonly ICustomerNotificationService _notificationService;

        public ProductCategoriesController(IProductCategoryService categoryProductService, ICustomerNotificationService notificationService)
        {
            _categoryProductService = categoryProductService;
            _notificationService = notificationService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryProductService.GetAll();
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryProductService.GetById(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCategoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _categoryProductService.Create(dto);
            await _notificationService.NotifyEntityChanged("ProductCategory");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductCategoryDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _categoryProductService.Update(id, dto);

            if (!updated)
                return NotFound();

            await _notificationService.NotifyEntityChanged("ProductCategory");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryProductService.Delete(id);

            if (!deleted)
                return NotFound();

            await _notificationService.NotifyEntityChanged("ProductCategory");
            return NoContent();
        }
    }
}
