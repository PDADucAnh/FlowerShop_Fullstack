using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliverySlotService _deliverySlotService;

        private static readonly List<object> SupportedDistricts = new()
        {
            new { Id = "q1", Name = "Quận 1" },
            new { Id = "q2", Name = "Quận 2" },
            new { Id = "q3", Name = "Quận 3" },
            new { Id = "q4", Name = "Quận 4" },
            new { Id = "q5", Name = "Quận 5" },
            new { Id = "q6", Name = "Quận 6" },
            new { Id = "q7", Name = "Quận 7" },
            new { Id = "q8", Name = "Quận 8" },
            new { Id = "q9", Name = "Quận 9" },
            new { Id = "q10", Name = "Quận 10" },
            new { Id = "q11", Name = "Quận 11" },
            new { Id = "q12", Name = "Quận 12" },
            new { Id = "bt", Name = "Bình Thạnh" },
            new { Id = "gv", Name = "Gò Vấp" },
            new { Id = "pn", Name = "Phú Nhuận" },
            new { Id = "tb", Name = "Tân Bình" },
            new { Id = "tp", Name = "Tân Phú" },
            new { Id = "td", Name = "Thủ Đức" },
            new { Id = "hb", Name = "Hóc Môn" },
            new { Id = "bc", Name = "Bình Chánh" }
        };

        public DeliveryController(IDeliverySlotService deliverySlotService)
        {
            _deliverySlotService = deliverySlotService;
        }

        [HttpGet("districts")]
        public IActionResult GetDistricts()
        {
            return Ok(SupportedDistricts);
        }

        [HttpGet("slots")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] int productId, [FromQuery] int daysAhead = 7)
        {
            var slots = await _deliverySlotService.GetAvailableSlots(productId, daysAhead);
            return Ok(slots);
        }

        [HttpGet("slots-by-district")]
        public async Task<IActionResult> GetSlotsByDistrict([FromQuery] string district, [FromQuery] string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest(new { message = "Ngày không hợp lệ" });

            var slots = await _deliverySlotService.GetAvailableSlotsByDistrict(district, parsedDate);
            return Ok(slots);
        }
    }
}
