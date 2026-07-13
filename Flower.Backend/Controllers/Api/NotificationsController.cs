using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetCustomerId()
        {
            var customerIdClaim = User.FindFirst("CustomerId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out int id))
            {
                return id;
            }
            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            int customerId = GetCustomerId();
            if (customerId == 0) return Unauthorized();

            var result = await _notificationService.GetCustomerNotifications(customerId, page, pageSize);
            
            return Ok(new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)System.Math.Ceiling((double)result.TotalCount / pageSize)
            });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            int customerId = GetCustomerId();
            if (customerId == 0) return Unauthorized();

            var count = await _notificationService.GetCustomerUnreadCount(customerId);
            return Ok(new { count });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            int customerId = GetCustomerId();
            if (customerId == 0) return Unauthorized();

            var result = await _notificationService.MarkAsRead(id, customerId);
            if (!result) return NotFound();

            return Ok(new { success = true });
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            int customerId = GetCustomerId();
            if (customerId == 0) return Unauthorized();

            await _notificationService.MarkAllAsRead(customerId);
            return Ok(new { success = true });
        }
    }
}
