using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDTO>> GetAll();
        Task<CouponDTO?> GetById(int id);
        Task<CouponDTO> Create(CreateCouponDTO dto);
        Task<bool> Update(int id, UpdateCouponDTO dto);
        Task<bool> Delete(int id);
        Task<bool> SetActive(int id, bool isActive);
        Task<ApplyCouponResponse> ValidateAndApply(ApplyCouponRequest request);
        Task<CouponUsageDTO?> GetUsageByOrderId(int orderId);
        Task<bool> ReleaseCoupon(int orderId);
        Task<IEnumerable<CouponUsageDTO>> GetUsagesByCoupon(int couponId);
    }
}
