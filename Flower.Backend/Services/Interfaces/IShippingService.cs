using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IShippingService
    {
        Task<decimal> CalculateShippingFee(decimal subtotal);
    }
}
