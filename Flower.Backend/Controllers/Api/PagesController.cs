using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PagesController(IPageService pageService)
        {
            _pageService = pageService;
        }

        [AllowAnonymous]
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var items = await _pageService.GetAllActive();
            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var page = await _pageService.GetBySlug(slug);
            if (page == null) return NotFound();
            return Ok(page);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _pageService.GetAll();
            return Ok(items);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var page = await _pageService.GetById(id);
            if (page == null) return NotFound();
            return Ok(page);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePageDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _pageService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePageDTO dto)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _pageService.Update(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _pageService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
