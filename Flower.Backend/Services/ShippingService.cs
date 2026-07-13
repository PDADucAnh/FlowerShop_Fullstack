using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class ShippingService : IShippingService
    {
        private readonly ISystemSettingService _settingService;

        public ShippingService(ISystemSettingService settingService)
        {
            _settingService = settingService;
        }

        public async Task<decimal> CalculateShippingFee(decimal subtotal)
        {
            var shippingSettings = await _settingService.GetSetting<ShippingSettings>("Shipping");
            if (shippingSettings == null)
            {
                return 30000; // Fallback default fee
            }

            if (subtotal >= shippingSettings.FreeShipFrom)
            {
                return 0;
            }

            return shippingSettings.DefaultFee;
        }
    }
}
