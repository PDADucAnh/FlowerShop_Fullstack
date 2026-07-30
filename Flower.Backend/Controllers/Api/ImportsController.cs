using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

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
        public async Task<IActionResult> Template()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "product_import_template.xlsx");
            if (System.IO.File.Exists(path))
            {
                var bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product_import_template.xlsx");
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Sản phẩm");
            sheet.Cells[1, 1].Value = "STT";
            sheet.Cells[1, 2].Value = "Mã sản phẩm";
            sheet.Cells[1, 3].Value = "Tên sản phẩm";
            sheet.Cells[1, 4].Value = "Mô tả";
            sheet.Cells[1, 5].Value = "Giá";
            sheet.Cells[1, 6].Value = "Số lượng";
            sheet.Cells[1, 7].Value = "Danh mục";
            sheet.Cells[1, 8].Value = "Trạng thái";
            sheet.Cells[1, 9].Value = "File ảnh";
            sheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;
            sheet.Cells[2, 1].Value = 1;
            sheet.Cells[2, 2].Value = "SP001";
            sheet.Cells[2, 3].Value = "Hoa hồng đỏ";
            sheet.Cells[2, 4].Value = "Hoa hồng nhập khẩu";
            sheet.Cells[2, 5].Value = 150000;
            sheet.Cells[2, 6].Value = 100;
            sheet.Cells[2, 7].Value = "Hoa tươi";
            sheet.Cells[2, 8].Value = "Active";
            sheet.Cells[2, 9].Value = "hoahong.jpg";
            sheet.Cells.AutoFitColumns();

            var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates");
            Directory.CreateDirectory(tempDir);
            var savePath = Path.Combine(tempDir, "product_import_template.xlsx");
            await System.IO.File.WriteAllBytesAsync(savePath, await package.GetAsByteArrayAsync());

            var bytes2 = await package.GetAsByteArrayAsync();
            return File(bytes2, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product_import_template.xlsx");
        }

        [HttpPost("categories/upload")]
        public async Task<IActionResult> UploadCategories(
            IFormFile excelFile,
            IFormFile? zipFile,
            [FromForm] string onDuplicate = "skip")
        {
            if (excelFile == null || excelFile.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file Excel" });

            var ext = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
            if (ext != ".xlsx")
                return BadRequest(new { message = "File Excel phải có định dạng .xlsx" });

            var result = await _importService.ImportCategoriesAsync(excelFile, zipFile, onDuplicate);

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

        [HttpGet("categories/template")]
        public async Task<IActionResult> CategoriesTemplate()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "category_import_template.xlsx");
            if (System.IO.File.Exists(path))
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(path);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "category_import_template.xlsx");
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Danh mục sản phẩm");
            sheet.Cells[1, 1].Value = "STT";
            sheet.Cells[1, 2].Value = "Tên danh mục";
            sheet.Cells[1, 3].Value = "Slug";
            sheet.Cells[1, 4].Value = "Mô tả";
            sheet.Cells[1, 5].Value = "File ảnh";
            sheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;
            sheet.Cells[2, 1].Value = 1;
            sheet.Cells[2, 2].Value = "Hoa sinh nhật";
            sheet.Cells[2, 3].Value = "hoa-sinh-nhat";
            sheet.Cells[2, 4].Value = "Danh mục hoa dành tặng sinh nhật";
            sheet.Cells[2, 5].Value = "birthday.jpg";
            sheet.Cells[3, 1].Value = 2;
            sheet.Cells[3, 2].Value = "Hoa cưới";
            sheet.Cells[3, 3].Value = "hoa-cuoi";
            sheet.Cells[3, 4].Value = "";
            sheet.Cells[3, 5].Value = "";
            sheet.Cells.AutoFitColumns();

            var tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates");
            Directory.CreateDirectory(tempDir);
            var savePath = Path.Combine(tempDir, "category_import_template.xlsx");

            var fileBytes = await package.GetAsByteArrayAsync();
            await System.IO.File.WriteAllBytesAsync(savePath, fileBytes);

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "category_import_template.xlsx");
        }
    }
}
