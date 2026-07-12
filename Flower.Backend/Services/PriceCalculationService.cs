using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Flower.Backend.Services
{
    public class PriceCalculationService : IPriceCalculationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPromotionService _promotionService;

        public PriceCalculationService(IApplicationDbContext context, IPromotionService promotionService)
        {
            _context = context;
            _promotionService = promotionService;
        }

        public async Task<CalculatedPriceDTO> CalculateProductPrice(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product {productId} not found");

            var result = new CalculatedPriceDTO
            {
                ProductId = productId,
                OriginalPrice = product.Price
            };

            var promotion = await _promotionService.GetBestPromotionForProduct(productId);
            if (promotion != null)
            {
                var discountAmount = promotion.DiscountType == DiscountType.Percent
                    ? product.Price * promotion.DiscountValue / 100m
                    : promotion.DiscountValue;

                result.PromotionPrice = product.Price - discountAmount;
                result.PromotionPercent = promotion.DiscountType == DiscountType.Percent ? promotion.DiscountValue : null;
                result.PromotionType = promotion.PromotionType.ToString();
                result.HasFlashSale = promotion.PromotionType == PromotionType.FlashSale;
                result.DiscountAmount = discountAmount;
            }

            return result;
        }

        public async Task<Dictionary<int, CalculatedPriceDTO>> CalculateBulkPrices(List<int> productIds)
        {
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var activePromotions = await _promotionService.GetActivePromotions();
            var result = new Dictionary<int, CalculatedPriceDTO>();

            foreach (var product in products)
            {
                var calc = new CalculatedPriceDTO
                {
                    ProductId = product.Id,
                    OriginalPrice = product.Price
                };

                var bestPromo = activePromotions
                    .Where(p => p.ProductIds.Contains(product.Id))
                    .OrderByDescending(p => p.Priority)
                    .FirstOrDefault();

                if (bestPromo != null)
                {
                    var discountAmount = bestPromo.DiscountType == DiscountType.Percent
                        ? product.Price * bestPromo.DiscountValue / 100m
                        : bestPromo.DiscountValue;

                    calc.PromotionPrice = product.Price - discountAmount;
                    calc.PromotionPercent = bestPromo.DiscountType == DiscountType.Percent ? bestPromo.DiscountValue : null;
                    calc.PromotionType = bestPromo.PromotionType.ToString();
                    calc.HasFlashSale = bestPromo.PromotionType == PromotionType.FlashSale;
                    calc.DiscountAmount = discountAmount;
                }

                result[product.Id] = calc;
            }

            return result;
        }

        public decimal CalculateCouponDiscount(decimal orderTotal, CouponDTO coupon)
        {
            if (coupon.DiscountType == DiscountType.Percent)
            {
                var discount = orderTotal * coupon.DiscountValue / 100m;
                if (coupon.MaximumDiscountAmount.HasValue && discount > coupon.MaximumDiscountAmount.Value)
                    discount = coupon.MaximumDiscountAmount.Value;
                return discount;
            }

            return coupon.DiscountValue > orderTotal ? orderTotal : coupon.DiscountValue;
        }
    }
}
