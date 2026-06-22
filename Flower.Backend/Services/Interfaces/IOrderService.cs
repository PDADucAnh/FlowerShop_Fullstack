using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAll();
        Task<OrderDTO?> GetDetail(int id);
        Task<(bool Success, string Message, int OrderId)> CreateOrder(int customerId, string? notes, List<OrderItemInput> items);
        Task<bool> Update(int id, UpdateOrderDTO dto);
        Task<bool> Delete(int id);
    }
}
