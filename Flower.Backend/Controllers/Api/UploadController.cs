using Flower.Backend.Helpers;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "StaffOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IPhotoService photoService, ILogger<UploadController> logger)
        {
            _photoService = photoService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string? folder = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file ảnh" });

            try
            {
                var validateStream = file.OpenReadStream();
                using var _ = Image.Load(validateStream);
            }
            catch
            {
                return BadRequest(new { message = "File không hợp lệ. Chỉ chấp nhận file ảnh." });
            }

            var url = await _photoService.UploadPhotoAsync(file, folder);
            if (string.IsNullOrEmpty(url))
            {
                return StatusCode(500, new { message = "Upload ảnh thất bại. Vui lòng thử lại." });
            }

            return Ok(new UploadImageResponse { Url = url });
        }
    }
}
