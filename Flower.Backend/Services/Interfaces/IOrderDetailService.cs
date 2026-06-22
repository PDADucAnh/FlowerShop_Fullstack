using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;

namespace Flower.Backend.Services.Interfaces
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailDTO>> GetAll();
        Task<OrderDetailDTO?> GetById(int id);
        Task<OrderDetailDTO> Create(OrderDetailDTO dto);
        Task<bool> Update(int id, OrderDetailDTO dto);
        Task<bool> Delete(int id);
        Task<IEnumerable<OrderDetailDTO>> GetByOrderId(int orderId);
    }
}
