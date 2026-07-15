using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly INotificationService _notificationService;

        public PromotionController(IPromotionService promotionService, INotificationService notificationService)
        {
            _promotionService = promotionService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _promotionService.GetAll();
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
        public async Task<IActionResult> Create(CreatePromotionCampaignDTO model, string? productIdsCsv)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(productIdsCsv))
            {
                model.ProductIds = productIdsCsv
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => { int.TryParse(s.Trim(), out var id); return id; })
                    .Where(id => id > 0)
                    .ToList();
            }

            try
            {
                await _promotionService.Create(model);
                await _notificationService.NotifyEntityChanged("PromotionCampaign");
                TempData["Success"] = "Đợt khuyến mãi đã được tạo thành công.";
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
            var item = await _promotionService.GetById(id);
            if (item == null) return NotFound();

            var model = new UpdatePromotionCampaignDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                PromotionType = item.PromotionType,
                DiscountType = item.DiscountType,
                DiscountValue = item.DiscountValue,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Priority = item.Priority,
                BannerImage = item.BannerImage,
                IsStackable = item.IsStackable,
                IsActive = item.IsActive,
                ProductIds = item.ProductIds
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(UpdatePromotionCampaignDTO model, string? productIdsCsv)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            var existing = await _promotionService.GetById(model.Id);
            if (existing == null)
            {
                TempData["Error"] = "Đợt khuyến mãi không tồn tại.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(productIdsCsv))
            {
                model.ProductIds = productIdsCsv
                    .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => { int.TryParse(s.Trim(), out var id); return id; })
                    .Where(id => id > 0)
                    .ToList();
            }
            else
            {
                model.ProductIds = new List<int>();
            }

            var updated = await _promotionService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật đợt khuyến mãi. Vui lòng thử lại.";
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("PromotionCampaign");
            TempData["Success"] = "Đợt khuyến mãi đã được cập nhật.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            await _promotionService.Delete(id);
            await _notificationService.NotifyEntityChanged("PromotionCampaign");
            TempData["Success"] = "Đợt khuyến mãi đã được xóa.";
            return RedirectToAction("Index");
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var item = await _promotionService.GetById(id);
            if (item == null) return NotFound();

            await _promotionService.SetActive(id, !item.IsActive);
            await _notificationService.NotifyEntityChanged("PromotionCampaign");
            TempData["Success"] = item.IsActive ? "Đã tắt đợt khuyến mãi." : "Đã bật đợt khuyến mãi.";
            return RedirectToAction("Index");
        }
    }
}
