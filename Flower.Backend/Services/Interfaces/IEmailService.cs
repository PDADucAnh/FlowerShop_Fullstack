using System.Threading.Tasks;
using Flower.Data.Entities;

namespace Flower.Backend.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(Order order, string customerEmail, string customerName);
        Task SendOrderConfirmedEmailAsync(Order order, string customerEmail, string customerName);
        Task SendOrderShippingEmailAsync(Order order, string customerEmail, string customerName);
        Task SendOrderCompletedEmailAsync(Order order, string customerEmail, string customerName);
        Task SendResetPasswordEmailAsync(string email, string name, string resetLink, string? rawToken = null);
        Task SendOtpEmailAsync(string email, string name, string otp);

        Task SendOrderCancelledByShopEmailAsync(Order order, string customerEmail, string customerName, string reason, decimal refundAmount);

        Task SendOrderCancelledByCustomerEmailAsync(Order order, string customerEmail, string customerName, decimal refundAmount);

        Task SendOrderCancelledWithFeeEmailAsync(Order order, string customerEmail, string customerName, decimal refundAmount, decimal feePercent, decimal feeAmount);

        Task SendOrderCannotCancelEmailAsync(Order order, string customerEmail, string customerName);

        Task SendRefundCompletedEmailAsync(Order order, string customerEmail, string customerName, decimal refundAmount, string? paymentMethod = null, string? refundTransactionId = null);
    }
}
