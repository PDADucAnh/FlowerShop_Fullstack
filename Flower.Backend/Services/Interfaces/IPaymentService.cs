using Flower.Backend.Models.DTOs;
using Flower.Data.Entities;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDTO> RecordPayment(int orderId, decimal amount, PaymentMethod method, string? transactionId = null);
        Task<int> CreatePendingPayment(int orderId, decimal amount, PaymentMethod method);
        Task<bool> ProcessWebhook(PaymentWebhookRequest request);
        Task<(bool Success, string Message)> ConfirmOnlinePayment(int orderId, string transactionId, decimal amount, string? ipAddress = null, string? userAgent = null, string? gatewayResponse = null);
        Task<bool> RefundPayment(int orderId, decimal amount);
        Task<PaymentDTO?> GetByOrderId(int orderId);
        Task<(bool Success, string Message, int PaymentId, string? PaymentUrl)> CreateRetryPayment(int orderId, PaymentMethod method);
        Task<(bool Success, string Message)> MarkPaymentFailed(int orderId, string? gatewayResponse = null, string? ipAddress = null, string? userAgent = null);
        Task<(bool Success, string Message)> ProcessRefund(int orderId, string? transactionId = null, string? responseCode = null, string? processedBy = null);
    }
}
