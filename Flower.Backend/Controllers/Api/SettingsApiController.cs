using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
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

        [HttpGet("store-info")]
        public async Task<IActionResult> GetStoreInfo()
        {
            var storeInfo = await _settingService.GetSetting<StoreInfoSettings>("StoreInfo");
            return Ok(storeInfo);
        }
    }
}
