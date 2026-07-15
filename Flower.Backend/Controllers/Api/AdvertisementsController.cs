using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly INotificationService _notificationService;

        public AdvertisementsController(IAdvertisementService advertisementService, INotificationService notificationService)
        {
            _advertisementService = advertisementService;
            _notificationService = notificationService;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var items = await _advertisementService.GetAllActive();
            return Ok(items);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _advertisementService.GetAll();
            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _advertisementService.GetById(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdvertisementDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _advertisementService.Create(dto);
            await _notificationService.NotifyEntityChanged("Advertisement");
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdvertisementDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _advertisementService.Update(id, dto);
            if (!updated)
                return NotFound();

            await _notificationService.NotifyEntityChanged("Advertisement");
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _advertisementService.Delete(id);
            if (!deleted)
                return NotFound();
            await _notificationService.NotifyEntityChanged("Advertisement");
            return NoContent();
        }
    }
}
