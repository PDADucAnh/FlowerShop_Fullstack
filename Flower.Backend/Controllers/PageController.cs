using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class PageController : Controller
    {
        private readonly IPageService _pageService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PageController> _logger;

        public PageController(IPageService pageService, INotificationService notificationService, ILogger<PageController> logger)
        {
            _pageService = pageService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _pageService.GetAll();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePageDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _pageService.Create(model);
            await _notificationService.NotifyEntityChanged("Page");
            TempData["Success"] = "Trang đã được tạo thành công.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _pageService.GetById(id);
            if (item == null) return NotFound();

            var model = new UpdatePageDTO
            {
                Id = item.Id,
                Title = item.Title,
                Slug = item.Slug,
                Content = item.Content,
                IsActive = item.IsActive
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePageDTO model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return View(model);
            }

            var updated = await _pageService.Update(model.Id, model);
            if (!updated)
            {
                TempData["Error"] = "Không thể cập nhật trang. Vui lòng thử lại.";
                return View(model);
            }

            await _notificationService.NotifyEntityChanged("Page");
            TempData["Success"] = "Trang đã được cập nhật.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _pageService.Delete(id);
            await _notificationService.NotifyEntityChanged("Page");
            TempData["Success"] = "Trang đã được xóa.";
            return RedirectToAction("Index");
        }
    }
}
