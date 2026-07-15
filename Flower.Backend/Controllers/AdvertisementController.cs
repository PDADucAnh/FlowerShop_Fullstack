using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class AdvertisementController : Controller
    {
        private readonly IAdvertisementService _advertisementService;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationService _notificationService;

        public AdvertisementController(IAdvertisementService advertisementService, IWebHostEnvironment env, INotificationService notificationService)
        {
            _advertisementService = advertisementService;
            _env = env;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _advertisementService.GetAll();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertisementDTO model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "herobars");
                Directory.CreateDirectory(uploadsDir);
                model.ImageUrl = await ImageHelper.SaveHeroImageAsync(imageFile, uploadsDir);
            }

            await _advertisementService.Create(model);
            await _notificationService.NotifyEntityChanged("Advertisement");
            TempData["Success"] = "Banner quảng cáo đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _advertisementService.GetById(id);
            if (item == null) return NotFound();

            var model = new UpdateAdvertisementDTO
            {
                Id = item.Id,
                Title = item.Title,
                Subtitle = item.Subtitle,
                ImageUrl = item.ImageUrl,
                LinkUrl = item.LinkUrl,
                SortOrder = item.SortOrder,
                IsActive = item.IsActive
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateAdvertisementDTO model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            var existing = await _advertisementService.GetById(model.Id);
            if (existing == null)
            {
                TempData["Error"] = "Banner không tồn tại.";
                return RedirectToAction("Index");
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                ImageHelper.DeleteImage(existing.ImageUrl, _env.WebRootPath);

                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "herobars");
                Directory.CreateDirectory(uploadsDir);
                model.ImageUrl = await ImageHelper.SaveHeroImageAsync(imageFile, uploadsDir);
            }
            else
            {
                model.ImageUrl = existing.ImageUrl;
            }

            var updated = await _advertisementService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật banner. Vui lòng thử lại.";
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("Advertisement");
            TempData["Success"] = "Banner quảng cáo đã được cập nhật.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _advertisementService.GetById(id);
            if (existing != null)
                ImageHelper.DeleteImage(existing.ImageUrl, _env.WebRootPath);

            await _advertisementService.Delete(id);
            await _notificationService.NotifyEntityChanged("Advertisement");
            TempData["Success"] = "Banner quảng cáo đã được xóa.";
            return RedirectToAction("Index");
        }
    }
}
