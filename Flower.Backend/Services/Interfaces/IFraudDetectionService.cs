using Flower.Data.Entities;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IFraudDetectionService
    {
        Task<bool> IsPhoneBlacklisted(string phoneNumber);
        Task<bool> RequiresVerification(Customer customer);
        Task<bool> VerifyOrder(int orderId, string otp);
        Task RecordFailedDelivery(string phoneNumber);
        Task BlacklistPhone(string phoneNumber, string reason);
        Task<int> CalculateFraudScore(Customer customer);
    }
}
