namespace Flower.Backend.Models.DTOs
{
    public class CalculatedPriceDTO
    {
        public int ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? PromotionPrice { get; set; }
        public decimal? PromotionPercent { get; set; }
        public string? PromotionType { get; set; }
        public bool HasFlashSale { get; set; }
        public decimal? DiscountAmount { get; set; }
    }
}
