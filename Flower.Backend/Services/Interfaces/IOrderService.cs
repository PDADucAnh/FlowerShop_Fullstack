using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flower.Backend.Models.DTOs;
using Flower.Data.Entities;

namespace Flower.Backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAll();
        Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize);
        Task<OrderDTO?> GetDetail(int id);
        Task<(bool Success, string Message, int OrderId)> CreateOrder(
            int customerId, string? notes, List<OrderItemInput> items,
            DateTime? orderDate = null, OrderStatus? status = null,
            PaymentMethod? paymentMethod = null, DateTime? deliveryDate = null,
            string? deliveryTimeSlot = null, string? deliveryDistrict = null,
            string? deliveryAddress = null,
            string? recipientName = null, string? recipientPhone = null);
        Task<bool> Update(int id, UpdateOrderDTO dto);
        Task<bool> Delete(int id);
        Task<bool> Cancel(int id);

        Task<(bool Success, string Message)> CancelWithPolicy(int id, string? reason = null);

        Task<bool> CancelWithReason(int id, string? reason);

        Task<(bool Success, string Message)> ProcessCODOrder(int orderId);

        Task<bool> IsPhoneBlacklisted(string phone);
    }
}
