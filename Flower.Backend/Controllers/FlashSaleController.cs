using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class FlashSaleController : Controller
    {
        private readonly IFlashSaleService _flashSaleService;
        private readonly INotificationService _notificationService;

        public FlashSaleController(IFlashSaleService flashSaleService, INotificationService notificationService)
        {
            _flashSaleService = flashSaleService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _flashSaleService.GetAll();
            return View(items);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create(CreateFlashSaleDTO model, string? productIdsCsv)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(productIdsCsv))
            {
                model.Products = productIdsCsv
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => { int.TryParse(s.Trim(), out var id); return id; })
                    .Where(id => id > 0)
                    .Select(id => new CreateFlashSaleProductDTO { ProductId = id, SalePrice = 0 })
                    .ToList();
            }

            try
            {
                await _flashSaleService.Create(model);
                await _notificationService.NotifyEntityChanged("FlashSale");
                TempData["Success"] = "Flash Sale đã được tạo thành công.";
                return RedirectToAction("Index");
            }
            catch (System.InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _flashSaleService.GetById(id);
            if (item == null) return NotFound();

            var model = new UpdateFlashSaleDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                IsActive = item.IsActive
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(UpdateFlashSaleDTO model, string? productIdsCsv)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            var existing = await _flashSaleService.GetById(model.Id);
            if (existing == null)
            {
                TempData["Error"] = "Flash Sale không tồn tại.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(productIdsCsv))
            {
                model.Products = productIdsCsv
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => { int.TryParse(s.Trim(), out var id); return id; })
                    .Where(id => id > 0)
                    .Select(id => new CreateFlashSaleProductDTO { ProductId = id, SalePrice = 0 })
                    .ToList();
            }
            else
            {
                model.Products = null;
            }

            var updated = await _flashSaleService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật Flash Sale. Vui lòng thử lại.";
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("FlashSale");
            TempData["Success"] = "Flash Sale đã được cập nhật.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _flashSaleService.Delete(id);
            if (!deleted)
            {
                TempData["Error"] = "Không thể xóa Flash Sale.";
                return RedirectToAction("Index");
            }
            await _notificationService.NotifyEntityChanged("FlashSale");
            TempData["Success"] = "Flash Sale đã được xóa.";
            return RedirectToAction("Index");
        }
    }
}
