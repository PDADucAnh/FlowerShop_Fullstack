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
    }
}
