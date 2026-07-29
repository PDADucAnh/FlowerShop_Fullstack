using System.ComponentModel.DataAnnotations;

namespace Flower.Backend.Models.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string? Sku { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryProductId { get; set; }
        public string? CategoryProductName { get; set; }
        public int ViewCount { get; set; }
        public int AddToCartCount { get; set; }
        public double TrendingScore { get; set; }
        public string? TrendingBadge { get; set; }
        public decimal? PromotionPrice { get; set; }
        public decimal? PromotionPercent { get; set; }
        public string? PromotionType { get; set; }
        public bool HasFlashSale { get; set; }
        public List<ProductImageDTO> Images { get; set; } = new();
        public bool IsActive { get; set; }
        public string? FlowerMeaning { get; set; }
        public string? Origin { get; set; }
        public string? CareInstruction { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool IsFlashSale { get; set; }
        public string? PromotionName { get; set; }
    }

    public class ProductImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
    }

    public class UploadImageResponse
    {
        public string Url { get; set; }
    }

    public class AddProductImageRequest
    {
        [Required]
        public string ImageUrl { get; set; }
    }

    public class CreateProductDTO
    {
        [MaxLength(50)]
        public string? Sku { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryProductId { get; set; }

        public bool IsActive { get; set; } = true;
        [MaxLength(500)]
        public string? FlowerMeaning { get; set; }
        [MaxLength(200)]
        public string? Origin { get; set; }
        public string? CareInstruction { get; set; }
        public List<string>? NewImages { get; set; }
    }

    public class UpdateProductDTO
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Sku { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MaxLength(200)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(300)]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm không được để trống")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryProductId { get; set; }

        public bool IsActive { get; set; } = true;
        [MaxLength(500)]
        public string? FlowerMeaning { get; set; }
        [MaxLength(200)]
        public string? Origin { get; set; }
        public string? CareInstruction { get; set; }
        public List<string>? NewImages { get; set; }
    }

    public class CartRecalculateRequest
    {
        public List<CartItemRecalculateDTO> Items { get; set; } = new List<CartItemRecalculateDTO>();
    }

    public class CartItemRecalculateDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
    }

    public class CartRecalculateResponse
    {
        public List<CartItemRecalculatedDTO> Items { get; set; } = new List<CartItemRecalculatedDTO>();
        public bool PriceChanged { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class CartItemRecalculatedDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool HasFlashSale { get; set; }
        public decimal? PromotionPercent { get; set; }
        public string? PromotionName { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public string? Description { get; set; }
    }
}
