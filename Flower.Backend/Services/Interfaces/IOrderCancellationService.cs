using System.Threading.Tasks;
using Flower.Data.Entities;

namespace Flower.Backend.Services.Interfaces
{
    public interface IOrderCancellationService
    {
        Task<bool> CancelWithReason(int id, string? reason);

        Task<(bool Success, string Message)> CancelByShop(int id, string? reason);

        Task<(bool Success, string Message)> CancelByCustomer(int id, string? reason);

        Task<(bool Success, string Message)> CancelWithPolicy(int id, string? reason, string cancelledBy);

        Task<CancellationPolicy?> GetPolicyForOrderStatus(OrderStatus status);

        Task SeedDefaultPoliciesAsync();
    }
}
