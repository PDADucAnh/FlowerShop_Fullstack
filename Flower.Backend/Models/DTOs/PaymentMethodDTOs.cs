namespace Flower.Backend.Models.DTOs
{
    public class PaymentMethodDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
