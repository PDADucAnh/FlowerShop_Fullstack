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
        Task<PagedResult<OrderDTO>> GetPaged(int page, int pageSize, List<OrderStatus>? statuses = null, string? search = null, DateTime? dateFrom = null, DateTime? dateTo = null, int? customerId = null);
        Task<OrderDTO?> GetDetail(int id);
        Task<(bool Success, string Message, int OrderId)> CreateOrder(
            int customerId, string? notes, List<OrderItemInput> items,
            DateTime? orderDate = null, OrderStatus? status = null,
            PaymentMethod? paymentMethod = null, DateTime? deliveryDate = null,
            string? deliveryTimeSlot = null, string? deliveryDistrict = null,
            string? deliveryAddress = null,
            string? recipientName = null, string? recipientPhone = null,
            string? couponCode = null);
        Task<bool> Update(int id, UpdateOrderDTO dto);
        Task<bool> Delete(int id);

        Task<(bool Success, string Message)> CancelByCustomer(int id, string? reason = null);

        Task<(bool Success, string Message)> CancelByShop(int id, string? reason = null);

        Task<bool> CancelWithReason(int id, string? reason);

        Task<bool> UpdateStatus(int id, OrderStatus newStatus);

        Task<(bool Success, string Message)> ProcessCODOrder(int orderId);

        Task<bool> IsPhoneBlacklisted(string phone);
    }
}
