namespace Flower.Backend.Models.DTOs
{
    public class VnPaymentRequestModel
    {
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string OrderDescription { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = "VnPay";
        public string PaymentId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string VnPayResponseCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
