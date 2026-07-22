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
    }
}
