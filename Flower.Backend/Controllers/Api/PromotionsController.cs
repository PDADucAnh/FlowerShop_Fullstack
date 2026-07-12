using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;
        private readonly ICouponService _couponService;

        public PromotionsController(IPromotionService promotionService, ICouponService couponService)
        {
            _promotionService = promotionService;
            _couponService = couponService;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var items = await _promotionService.GetActivePromotions();
            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetBestForProduct(int productId)
        {
            var item = await _promotionService.GetBestPromotionForProduct(productId);
            if (item == null) return Ok(new { });
            return Ok(item);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _promotionService.GetAll();
            return Ok(items);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _promotionService.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromotionCampaignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var item = await _promotionService.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePromotionCampaignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _promotionService.Update(id, dto);
                if (!result) return NotFound();
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
            var result = await _promotionService.Delete(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{id}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var result = await _promotionService.SetActive(id, true);
            if (!result) return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var result = await _promotionService.SetActive(id, false);
            if (!result) return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost("{id}/products")]
        public async Task<IActionResult> AddProduct(int id, [FromBody] AddProductRequest request)
        {
            var result = await _promotionService.AddProductToPromotion(id, request.ProductId);
            if (!result) return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{id}/products/{productId}")]
        public async Task<IActionResult> RemoveProduct(int id, int productId)
        {
            var result = await _promotionService.RemoveProductFromPromotion(id, productId);
            if (!result) return NotFound();
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _couponService.ValidateAndApply(request);
            return Ok(result);
        }
    }

    public class AddProductRequest
    {
        public int ProductId { get; set; }
    }
}
