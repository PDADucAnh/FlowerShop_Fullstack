using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Utils;
using Microsoft.EntityFrameworkCore;

namespace Flower.Backend.Services
{
    public class CouponService : ICouponService
    {
        private readonly IApplicationDbContext _context;

        public CouponService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CouponDTO>> GetAll()
        {
            var coupons = await _context.Coupons
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return coupons.Select(c => c.ToDTO());
        }

        public async Task<CouponDTO?> GetById(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            return coupon?.ToDTO();
        }

        public async Task<CouponDTO> Create(CreateCouponDTO dto)
        {
            var exists = await _context.Coupons.AnyAsync(c => c.Code == dto.Code);
            if (exists)
                throw new InvalidOperationException($"Mã giảm giá '{dto.Code}' đã tồn tại");
            if (dto.DiscountValue < 0)
                throw new InvalidOperationException("DiscountValue không được âm");
            if (dto.DiscountType == DiscountType.Percent && dto.DiscountValue > 100)
                throw new InvalidOperationException("DiscountPercent không được vượt quá 100%");

            var coupon = dto.ToEntity();
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon.ToDTO();
        }

        public async Task<bool> Update(int id, UpdateCouponDTO dto)
        {
            if (id != dto.Id) return false;

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            var duplicate = await _context.Coupons.AnyAsync(c => c.Code == dto.Code && c.Id != id);
            if (duplicate)
                throw new InvalidOperationException($"Mã giảm giá '{dto.Code}' đã tồn tại");
            if (dto.DiscountValue < 0)
                throw new InvalidOperationException("DiscountValue không được âm");
            if (dto.DiscountType == DiscountType.Percent && dto.DiscountValue > 100)
                throw new InvalidOperationException("DiscountPercent không được vượt quá 100%");

            dto.UpdateEntity(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            var hasUsages = await _context.CouponUsages.AnyAsync(cu => cu.CouponId == id);
            if (hasUsages)
                throw new InvalidOperationException("Không thể xóa coupon đã được sử dụng");

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetActive(int id, bool isActive)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return false;

            coupon.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ApplyCouponResponse> ValidateAndApply(ApplyCouponRequest request)
        {
            var now = DateTimeUtils.GetVietnamTime();

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == request.Code);

            if (coupon == null)
                return new ApplyCouponResponse { IsValid = false, Message = "Mã giảm giá không tồn tại" };

            if (!coupon.IsActive)
                return new ApplyCouponResponse { IsValid = false, Message = "Mã giảm giá đã bị khóa" };

            if (coupon.StartDate.HasValue && coupon.StartDate.Value > now)
                return new ApplyCouponResponse { IsValid = false, Message = "Mã giảm giá chưa đến hạn sử dụng" };

            if (coupon.EndDate.HasValue && coupon.EndDate.Value < now)
                return new ApplyCouponResponse { IsValid = false, Message = "Mã giảm giá đã hết hạn" };

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                return new ApplyCouponResponse { IsValid = false, Message = "Mã giảm giá đã hết lượt sử dụng" };

            if (coupon.UsagePerCustomer.HasValue)
            {
                var customerUsage = await _context.CouponUsages
                    .CountAsync(cu => cu.CouponId == coupon.Id && cu.CustomerId == request.CustomerId);
                if (customerUsage >= coupon.UsagePerCustomer.Value)
                    return new ApplyCouponResponse { IsValid = false, Message = "Bạn đã dùng hết lượt cho mã này" };
            }

            if (coupon.CustomerId.HasValue && coupon.CustomerId.Value != request.CustomerId)
                return new ApplyCouponResponse { IsValid = false, Message = "Mã giảm giá không dành cho bạn" };

            if (coupon.MinimumOrderAmount.HasValue && request.OrderTotal < coupon.MinimumOrderAmount.Value)
                return new ApplyCouponResponse
                {
                    IsValid = false,
                    Message = $"Đơn hàng tối thiểu {coupon.MinimumOrderAmount.Value:N0}đ để sử dụng mã này"
                };

            var discountAmount = coupon.DiscountType == DiscountType.Percent
                ? request.OrderTotal * coupon.DiscountValue / 100m
                : coupon.DiscountValue;

            if (coupon.MaximumDiscountAmount.HasValue && discountAmount > coupon.MaximumDiscountAmount.Value)
                discountAmount = coupon.MaximumDiscountAmount.Value;

            if (discountAmount > request.OrderTotal)
                discountAmount = request.OrderTotal;

            return new ApplyCouponResponse
            {
                IsValid = true,
                Message = "Áp dụng mã giảm giá thành công",
                DiscountAmount = discountAmount,
                FinalTotal = request.OrderTotal - discountAmount,
                Coupon = coupon.ToDTO()
            };
        }

        public async Task<CouponUsageDTO?> GetUsageByOrderId(int orderId)
        {
            var usage = await _context.CouponUsages
                .Include(cu => cu.Coupon)
                .Include(cu => cu.Customer)
                .FirstOrDefaultAsync(cu => cu.OrderId == orderId);
            return usage?.ToDTO();
        }

        public async Task<bool> ReleaseCoupon(int orderId)
        {
            var usage = await _context.CouponUsages
                .Include(cu => cu.Coupon)
                .FirstOrDefaultAsync(cu => cu.OrderId == orderId);
            if (usage == null) return false;

            if (usage.Coupon != null)
            {
                usage.Coupon.UsedCount = Math.Max(0, usage.Coupon.UsedCount - 1);
            }

            _context.CouponUsages.Remove(usage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CouponUsageDTO>> GetUsagesByCoupon(int couponId)
        {
            var usages = await _context.CouponUsages
                .Include(cu => cu.Coupon)
                .Include(cu => cu.Customer)
                .Where(cu => cu.CouponId == couponId)
                .OrderByDescending(cu => cu.UsedAt)
                .ToListAsync();
            return usages.Select(u => u.ToDTO());
        }
    }
}
