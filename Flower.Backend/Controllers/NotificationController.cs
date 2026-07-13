using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers
{
    [Authorize(Policy = "StaffOnly")]
    public class NotificationController : Controller
    {
        private readonly IAdminNotificationService _notificationService;

        public NotificationController(IAdminNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: Notification
        public async Task<IActionResult> Index(string? type, string? search, int page = 1, int pageSize = 10)
        {
            var result = await _notificationService.GetAllNotifications(type, search, page, pageSize);
            var notifications = result.Items;
            var totalCount = result.TotalCount;
            
            ViewBag.Type = type;
            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)System.Math.Ceiling((double)totalCount / pageSize);

            return View(notifications);
        }

        // AJAX: Unread count
        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCount();
            return Json(new { count });
        }

        // AJAX: Latest notifications for dropdown
        [HttpGet]
        public async Task<IActionResult> GetLatest()
        {
            var notifications = await _notificationService.GetLatestNotifications();
            return Json(notifications);
        }

        // AJAX: Mark as read
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsRead(id);
            return Json(new { success = true });
        }

        // AJAX: Mark all as read
        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsRead();
            return Json(new { success = true });
        }

        // POST: Notification/SimulateReview
        [HttpPost]
        public async Task<IActionResult> SimulateReview(string customerName, string productName, int rating, string comment)
        {
            await _notificationService.CreateNotification(
                "Đánh giá mới",
                $"Khách hàng {customerName} đã đánh giá {rating} sao cho sản phẩm '{productName}': \"{comment}\"",
                "Review"
            );
            return Json(new { success = true, message = "Đã mô phỏng đánh giá mới thành công!" });
        }
    }
}
