using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Flower.Backend.Controllers.Api
{
    [Route("api/settings")]
    [ApiController]
    public class SettingsApiController : ControllerBase
    {
        private readonly ISystemSettingService _settingService;

        public SettingsApiController(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        [AllowAnonymous]
        [HttpGet("store-info")]
        public async Task<IActionResult> GetStoreInfo()
        {
            var storeInfo = await _settingService.GetSetting<StoreInfoSettings>("StoreInfo");
            return Ok(storeInfo);
        }

        [AllowAnonymous]
        [HttpGet("checkout")]
        public async Task<IActionResult> GetCheckoutSettings()
        {
            var shipping = await _settingService.GetSetting<ShippingSettings>("Shipping") ?? new ShippingSettings();
            var order = await _settingService.GetSetting<OrderSettings>("Order") ?? new OrderSettings();
            return Ok(new { shipping, order });
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var settings = await _settingService.GetAllSettings();
            return Ok(settings);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("store-info")]
        public async Task<IActionResult> SaveStoreInfo([FromBody] StoreInfoSettings dto)
        {
            var username = User.Identity?.Name ?? "System";
            await _settingService.SaveSetting("StoreInfo", dto, username);
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("smtp")]
        public async Task<IActionResult> SaveSmtp([FromBody] SmtpSettings dto)
        {
            var username = User.Identity?.Name ?? "System";
            await _settingService.SaveSetting("Smtp", dto, username);
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("vnpay")]
        public async Task<IActionResult> SaveVnPay([FromBody] VNPaySettings dto)
        {
            var username = User.Identity?.Name ?? "System";
            await _settingService.SaveSetting("VNPay", dto, username);
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("shipping")]
        public async Task<IActionResult> SaveShipping([FromBody] ShippingSettings dto)
        {
            var username = User.Identity?.Name ?? "System";
            await _settingService.SaveSetting("Shipping", dto, username);
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("order")]
        public async Task<IActionResult> SaveOrder([FromBody] OrderSettings dto)
        {
            var username = User.Identity?.Name ?? "System";
            await _settingService.SaveSetting("Order", dto, username);
            return NoContent();
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPut("cloudinary")]
        public async Task<IActionResult> SaveCloudinary([FromBody] CloudinarySettings dto)
        {
            var username = User.Identity?.Name ?? "System";
            await _settingService.SaveSetting("Cloudinary", dto, username);
            return NoContent();
        }
    }
}
