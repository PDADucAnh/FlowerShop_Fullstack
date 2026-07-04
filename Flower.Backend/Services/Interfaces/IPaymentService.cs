using Flower.Backend.Models.DTOs;
using Flower.Data.Entities;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDTO> RecordPayment(int orderId, decimal amount, PaymentMethod method, string? transactionId = null);
        Task<bool> ProcessWebhook(PaymentWebhookRequest request);
        Task<bool> RefundPayment(int orderId, decimal amount);
        Task<PaymentDTO?> GetByOrderId(int orderId);
    }
}
