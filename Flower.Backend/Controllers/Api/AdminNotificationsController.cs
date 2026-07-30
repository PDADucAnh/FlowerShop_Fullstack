using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "StaffOnly")]
    [Route("api/admin-notifications")]
    [ApiController]
    public class AdminNotificationsController : ControllerBase
    {
        private readonly IAdminNotificationService _adminNotificationService;

        public AdminNotificationsController(IAdminNotificationService adminNotificationService)
        {
            _adminNotificationService = adminNotificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? type, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _adminNotificationService.GetAllNotifications(type, search, page, pageSize);
            return Ok(new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)System.Math.Ceiling((double)result.TotalCount / pageSize)
            });
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest([FromQuery] int limit = 10)
        {
            var notifications = await _adminNotificationService.GetLatestNotifications(limit);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _adminNotificationService.GetUnreadCount();
            return Ok(new { count });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _adminNotificationService.MarkAsRead(id);
            return Ok(new { success = true });
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _adminNotificationService.MarkAllAsRead();
            return Ok(new { success = true });
        }
    }
}
