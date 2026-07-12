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
    public class PromotionService : IPromotionService
    {
        private readonly IApplicationDbContext _context;

        public PromotionService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PromotionCampaignDTO>> GetAll()
        {
            var campaigns = await _context.PromotionCampaigns
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return campaigns.Select(p => p.ToDTO());
        }

        public async Task<PromotionCampaignDTO?> GetById(int id)
        {
            var campaign = await _context.PromotionCampaigns
                .Include(p => p.PromotionProducts)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (campaign == null) return null;

            var dto = campaign.ToDTO();
            if (campaign.PromotionProducts != null)
            {
                dto.ProductIds = campaign.PromotionProducts.Select(pp => pp.ProductId).ToList();
            }
            return dto;
        }

        public async Task<PromotionCampaignDTO> Create(CreatePromotionCampaignDTO dto)
        {
            if (dto.EndDate <= dto.StartDate)
                throw new InvalidOperationException("EndDate phải lớn hơn StartDate");
            if (dto.DiscountValue < 0)
                throw new InvalidOperationException("DiscountValue không được âm");
            if (dto.DiscountType == DiscountType.Percent && dto.DiscountValue > 100)
                throw new InvalidOperationException("DiscountPercent không được vượt quá 100%");

            var campaign = dto.ToEntity();
            _context.PromotionCampaigns.Add(campaign);
            await _context.SaveChangesAsync();

            if (dto.ProductIds != null)
            {
                foreach (var productId in dto.ProductIds)
                {
                    _context.PromotionProducts.Add(new PromotionProduct
                    {
                        PromotionId = campaign.Id,
                        ProductId = productId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return campaign.ToDTO();
        }

        public async Task<bool> Update(int id, UpdatePromotionCampaignDTO dto)
        {
            if (id != dto.Id) return false;

            var campaign = await _context.PromotionCampaigns
                .Include(p => p.PromotionProducts)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (campaign == null) return false;

            if (dto.EndDate <= dto.StartDate)
                throw new InvalidOperationException("EndDate phải lớn hơn StartDate");
            if (dto.DiscountValue < 0)
                throw new InvalidOperationException("DiscountValue không được âm");
            if (dto.DiscountType == DiscountType.Percent && dto.DiscountValue > 100)
                throw new InvalidOperationException("DiscountPercent không được vượt quá 100%");

            dto.UpdateEntity(campaign);

            if (dto.ProductIds != null)
            {
                var existingIds = campaign.PromotionProducts?.Select(pp => pp.ProductId).ToList() ?? new List<int>();
                var toRemove = existingIds.Except(dto.ProductIds).ToList();
                var toAdd = dto.ProductIds.Except(existingIds).ToList();

                if (campaign.PromotionProducts != null)
                {
                    foreach (var pp in campaign.PromotionProducts.Where(pp => toRemove.Contains(pp.ProductId)).ToList())
                    {
                        _context.PromotionProducts.Remove(pp);
                    }
                }

                foreach (var productId in toAdd)
                {
                    _context.PromotionProducts.Add(new PromotionProduct
                    {
                        PromotionId = campaign.Id,
                        ProductId = productId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var campaign = await _context.PromotionCampaigns.FindAsync(id);
            if (campaign == null) return false;

            var products = await _context.PromotionProducts
                .Where(pp => pp.PromotionId == id)
                .ToListAsync();
            _context.PromotionProducts.RemoveRange(products);
            _context.PromotionCampaigns.Remove(campaign);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetActive(int id, bool isActive)
        {
            var campaign = await _context.PromotionCampaigns.FindAsync(id);
            if (campaign == null) return false;

            campaign.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddProductToPromotion(int promotionId, int productId)
        {
            var exists = await _context.PromotionProducts
                .AnyAsync(pp => pp.PromotionId == promotionId && pp.ProductId == productId);
            if (exists) return true;

            _context.PromotionProducts.Add(new PromotionProduct
            {
                PromotionId = promotionId,
                ProductId = productId
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveProductFromPromotion(int promotionId, int productId)
        {
            var pp = await _context.PromotionProducts
                .FirstOrDefaultAsync(p => p.PromotionId == promotionId && p.ProductId == productId);
            if (pp == null) return false;

            _context.PromotionProducts.Remove(pp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ActivePromotionDTO>> GetActivePromotions()
        {
            var now = DateTimeUtils.GetVietnamTime();
            var campaigns = await _context.PromotionCampaigns
                .Include(p => p.PromotionProducts)
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .OrderByDescending(p => p.Priority)
                .ToListAsync();

            return campaigns.Select(c => new ActivePromotionDTO
            {
                PromotionId = c.Id,
                Name = c.Name,
                PromotionType = c.PromotionType,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                Priority = c.Priority,
                IsStackable = c.IsStackable,
                BannerImage = c.BannerImage,
                ProductIds = c.PromotionProducts?.Select(pp => pp.ProductId).ToList() ?? new List<int>()
            });
        }

        public async Task<ActivePromotionDTO?> GetBestPromotionForProduct(int productId)
        {
            var now = DateTimeUtils.GetVietnamTime();
            var campaign = await _context.PromotionCampaigns
                .Include(p => p.PromotionProducts)
                .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now
                    && p.PromotionProducts != null && p.PromotionProducts.Any(pp => pp.ProductId == productId))
                .OrderByDescending(p => p.Priority)
                .FirstOrDefaultAsync();

            if (campaign == null) return null;

            return new ActivePromotionDTO
            {
                PromotionId = campaign.Id,
                Name = campaign.Name,
                PromotionType = campaign.PromotionType,
                DiscountType = campaign.DiscountType,
                DiscountValue = campaign.DiscountValue,
                Priority = campaign.Priority,
                IsStackable = campaign.IsStackable,
                BannerImage = campaign.BannerImage,
                ProductIds = campaign.PromotionProducts?.Select(pp => pp.ProductId).ToList() ?? new List<int>()
            };
        }

        public async Task AutoActivateExpired()
        {
            var now = DateTimeUtils.GetVietnamTime();

            var toActivate = await _context.PromotionCampaigns
                .Where(p => !p.IsActive && p.StartDate <= now && p.EndDate >= now)
                .ToListAsync();

            var toDeactivate = await _context.PromotionCampaigns
                .Where(p => p.IsActive && p.EndDate < now)
                .ToListAsync();

            foreach (var c in toActivate)
            {
                c.IsActive = true;
            }

            foreach (var c in toDeactivate)
            {
                c.IsActive = false;
            }

            if (toDeactivate.Count > 0 || toActivate.Count > 0)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
