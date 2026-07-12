using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPriceCalculationService
    {
        Task<CalculatedPriceDTO> CalculateProductPrice(int productId);
        Task<Dictionary<int, CalculatedPriceDTO>> CalculateBulkPrices(List<int> productIds);
        decimal CalculateCouponDiscount(decimal orderTotal, CouponDTO coupon);
    }
}
