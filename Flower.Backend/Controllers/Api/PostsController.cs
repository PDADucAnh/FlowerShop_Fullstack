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
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly INotificationService _notificationService;

        public PostsController(IPostService postService, INotificationService notificationService)
        {
            _postService = postService;
            _notificationService = notificationService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetAll();
            return Ok(posts);
        }

        [AllowAnonymous]
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var posts = await _postService.GetByCategory(categoryId);
            return Ok(posts);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var post = await _postService.GetById(id);

            if (post == null)
            {
                return NotFound(new { message = "Không tìm thấy bài viết này trong hệ thống" });
            }

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _postService.Create(dto);
            await _notificationService.NotifyEntityChanged("Post");
            return CreatedAtAction(nameof(GetDetail), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePostDTO dto)
        {
            if (id != dto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _postService.Update(id, dto);

            if (!updated)
                return NotFound();

            await _notificationService.NotifyEntityChanged("Post");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _postService.Delete(id);

            if (!deleted)
                return NotFound();

            await _notificationService.NotifyEntityChanged("Post");
            return NoContent();
        }
    }
}
