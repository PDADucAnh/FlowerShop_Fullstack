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
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _couponService.GetAll();
            return Ok(items);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _couponService.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCouponDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var item = await _couponService.Create(dto);
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCouponDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _couponService.Update(id, dto);
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
            try
            {
                var result = await _couponService.Delete(id);
                if (!result) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            var result = await _couponService.SetActive(id, true);
            if (!result) return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            var result = await _couponService.SetActive(id, false);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ApplyCouponRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _couponService.ValidateAndApply(request);
            return Ok(result);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyCouponRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _couponService.ValidateAndApply(request);
            return Ok(result);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("{id}/usages")]
        public async Task<IActionResult> GetUsages(int id)
        {
            var items = await _couponService.GetUsagesByCoupon(id);
            return Ok(items);
        }
    }
}
