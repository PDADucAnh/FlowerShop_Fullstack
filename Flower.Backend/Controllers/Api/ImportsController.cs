using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "StaffOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ImportsController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportsController(IImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            IFormFile excelFile,
            IFormFile? zipFile,
            [FromForm] string onDuplicate = "skip")
        {
            if (excelFile == null || excelFile.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file Excel" });

            var ext = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
            if (ext != ".xlsx")
                return BadRequest(new { message = "File Excel phải có định dạng .xlsx" });

            var result = await _importService.ImportProductsAsync(excelFile, zipFile, onDuplicate);

            var response = new ImportApiResponse
            {
                TotalRows = result.TotalRows,
                SuccessCount = result.SuccessCount,
                FailureCount = result.FailureCount,
                Errors = result.Errors,
                SkippedSkus = result.SkippedSkus
            };

            return Ok(response);
        }

        [HttpGet("template")]
        public IActionResult Template()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "product_import_template.xlsx");
            if (!System.IO.File.Exists(path))
                return NotFound(new { message = "File template không tồn tại" });

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product_import_template.xlsx");
        }
    }
}
