using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IOrderCancellationService
    {
        Task<bool> CancelWithReason(int id, string? reason);
    }
}
