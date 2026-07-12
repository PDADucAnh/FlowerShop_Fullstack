using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flower.Data.Entities
{
    public enum PromotionType
    {
        FlashSale = 0,
        Seasonal = 1
    }

    public enum DiscountType
    {
        Percent = 0,
        FixedAmount = 1
    }

    public class PromotionCampaign
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public PromotionType PromotionType { get; set; }

        public DiscountType DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountValue { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Priority { get; set; }

        [MaxLength(1000)]
        public string? BannerImage { get; set; }

        public bool IsStackable { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<PromotionProduct>? PromotionProducts { get; set; }
    }
}
