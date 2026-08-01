using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashSalesController : ControllerBase
    {
        private readonly IFlashSaleService _flashSaleService;
        private readonly ICustomerNotificationService _notificationService;

        public FlashSalesController(IFlashSaleService flashSaleService, ICustomerNotificationService notificationService)
        {
            _flashSaleService = flashSaleService;
            _notificationService = notificationService;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var items = await _flashSaleService.GetActiveFlashSales();
            return Ok(items);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _flashSaleService.GetAll();
            return Ok(items);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _flashSaleService.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFlashSaleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var item = await _flashSaleService.Create(dto);
                await _notificationService.NotifyEntityChanged("FlashSale");
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFlashSaleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _flashSaleService.Update(id, dto);
                if (!result) return NotFound();
                await _notificationService.NotifyEntityChanged("FlashSale");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _flashSaleService.Delete(id);
            if (!result) return NotFound();
            await _notificationService.NotifyEntityChanged("FlashSale");
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("preview/category")]
        public async Task<IActionResult> PreviewByCategory([FromBody] FlashSalePreviewRequestDto dto)
        {
            try
            {
                var items = await _flashSaleService.PreviewByCategory(dto);
                return Ok(items);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("preview/bestseller")]
        public async Task<IActionResult> PreviewByBestSeller([FromBody] FlashSalePreviewRequestDto dto)
        {
            try
            {
                var items = await _flashSaleService.PreviewByBestSeller(dto);
                return Ok(items);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("preview/excel")]
        public async Task<IActionResult> PreviewByExcel([FromForm] int flashSaleId, [FromForm] decimal? defaultDiscountPercent, [FromForm] IFormFile? file)
        {
            try
            {
                var items = await _flashSaleService.PreviewByExcel(flashSaleId, defaultDiscountPercent, file!);
                return Ok(items);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("bulk-add")]
        public async Task<IActionResult> BulkAdd([FromBody] BulkAddFlashSaleProductsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var count = await _flashSaleService.BulkAdd(dto);
                await _notificationService.NotifyEntityChanged("FlashSale");
                return Ok(new { added = count });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
