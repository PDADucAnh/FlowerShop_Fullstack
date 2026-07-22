using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _contactService.Create(dto);
            return Ok(new { success = true, message = "Cảm ơn bạn đã gửi liên hệ. Chúng tôi sẽ phản hồi sớm nhất." });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _contactService.GetAll();
            return Ok(items);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _contactService.GetUnreadCount();
            return Ok(new { count });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _contactService.GetById(id);
            if (contact == null) return NotFound();
            return Ok(contact);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(int id, [FromBody] MarkReadContactDTO dto)
        {
            var updated = await _contactService.MarkRead(id, dto.IsRead);
            if (!updated) return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _contactService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
