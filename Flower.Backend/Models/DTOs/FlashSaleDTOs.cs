using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class FlashSaleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<FlashSaleProductDTO>? Products { get; set; }
    }

    public class FlashSaleProductDTO
    {
        public int Id { get; set; }
        public int FlashSaleId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscountPercent { get; set; }
    }

    public class FlashSaleActiveDTO
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public string PromotionName { get; set; } = string.Empty;
        public DateTime PromotionEndTime { get; set; }
    }

    public class CreateFlashSaleDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public List<CreateFlashSaleProductDTO>? Products { get; set; }
    }

    public class CreateFlashSaleProductDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal SalePrice { get; set; }
    }

    public class UpdateFlashSaleDTO
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string? Name { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public List<CreateFlashSaleProductDTO>? Products { get; set; }
    }
}
