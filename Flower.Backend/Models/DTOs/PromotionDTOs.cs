using Flower.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class PromotionCampaignDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public PromotionType PromotionType { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public string? BannerImage { get; set; }
        public bool IsStackable { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<int>? ProductIds { get; set; }
    }

    public class CreatePromotionCampaignDTO
    {
        [Required(ErrorMessage = "Tên chiến dịch không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public PromotionType PromotionType { get; set; }

        public DiscountType DiscountType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int Priority { get; set; }

        [MaxLength(1000)]
        public string? BannerImage { get; set; }

        public bool IsStackable { get; set; }

        public bool IsActive { get; set; } = true;

        public List<int>? ProductIds { get; set; }
    }

    public class UpdatePromotionCampaignDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên chiến dịch không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public PromotionType PromotionType { get; set; }

        public DiscountType DiscountType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int Priority { get; set; }

        [MaxLength(1000)]
        public string? BannerImage { get; set; }

        public bool IsStackable { get; set; }

        public bool IsActive { get; set; } = true;

        public List<int>? ProductIds { get; set; }
    }

    public class ActivePromotionDTO
    {
        public int PromotionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public PromotionType PromotionType { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public int Priority { get; set; }
        public bool IsStackable { get; set; }
        public string? BannerImage { get; set; }
        public List<int> ProductIds { get; set; } = new();
    }

    public class PromotionProductDTO
    {
        public int Id { get; set; }
        public int PromotionId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
