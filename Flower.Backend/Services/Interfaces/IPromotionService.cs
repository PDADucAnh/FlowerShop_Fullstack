using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionCampaignDTO>> GetAll();
        Task<PromotionCampaignDTO?> GetById(int id);
        Task<PromotionCampaignDTO> Create(CreatePromotionCampaignDTO dto);
        Task<bool> Update(int id, UpdatePromotionCampaignDTO dto);
        Task<bool> Delete(int id);
        Task<bool> SetActive(int id, bool isActive);
        Task<bool> AddProductToPromotion(int promotionId, int productId);
        Task<bool> RemoveProductFromPromotion(int promotionId, int productId);
        Task<IEnumerable<ActivePromotionDTO>> GetActivePromotions();
        Task<ActivePromotionDTO?> GetBestPromotionForProduct(int productId);
        Task AutoActivateExpired();
    }
}
