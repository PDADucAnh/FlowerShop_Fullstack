using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flower.Backend.Controllers;

[Authorize(Policy = "StaffOnly")]
public class ImportController : Controller
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ImportViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        IFormFile excelFile,
        IFormFile? zipFile,
        string onDuplicate)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            TempData["Error"] = "Vui lòng chọn file Excel";
            return View(new ImportViewModel());
        }

        var result = await _importService.ImportProductsAsync(excelFile, zipFile, onDuplicate ?? "skip");
        return View(new ImportViewModel { Result = result });
    }

    [HttpGet]
    public IActionResult DownloadTemplate()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "product_import_template.xlsx");
        if (!System.IO.File.Exists(path))
            return NotFound("File template không tồn tại");
        return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "product_import_template.xlsx");
    }
}
