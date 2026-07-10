using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var data = await _dashboardService.GetSummary();
            return Ok(data);
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        {
            var data = await _dashboardService.GetRevenue();
            return Ok(data);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var data = await _dashboardService.GetOrders();
            return Ok(data);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var data = await _dashboardService.GetProducts();
            return Ok(data);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var data = await _dashboardService.GetCustomers();
            return Ok(data);
        }

        [HttpGet("charts")]
        public async Task<IActionResult> GetCharts()
        {
            var data = await _dashboardService.GetCharts();
            return Ok(data);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var data = await _dashboardService.GetNotifications();
            return Ok(data);
        }
    }
}
