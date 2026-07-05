using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class OrderCancellationService : IOrderCancellationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IDeliverySlotService _deliverySlotService;
        private readonly StockLockService _stockLockService;

        public OrderCancellationService(
            IApplicationDbContext context,
            IDeliverySlotService deliverySlotService,
            StockLockService stockLockService)
        {
            _context = context;
            _deliverySlotService = deliverySlotService;
            _stockLockService = stockLockService;
        }

        public async Task<bool> CancelWithReason(int id, string? reason)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return false;

            if (order.Status == OrderStatus.Cancelled) return true;

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.Now;
            order.CancellationReason = reason;

            bool wasDeducted = order.PaymentMethod == PaymentMethod.COD
                || (order.PaymentMethod == PaymentMethod.OnlinePayment && order.PaymentStatus == PaymentStatus.Completed);

            if (order.OrderDetails != null)
            {
                var productIds = order.OrderDetails.Select(od => od.ProductId).ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var detail in order.OrderDetails)
                {
                    if (wasDeducted)
                    {
                        var product = products.FirstOrDefault(p => p.Id == detail.ProductId);
                        if (product != null)
                            product.StockQuantity += detail.Quantity;
                    }

                    if (!string.IsNullOrEmpty(order.DeliveryTimeSlot) && order.DeliveryDate.HasValue)
                        await _deliverySlotService.ReleaseSlot(detail.ProductId, order.DeliveryDate.Value, order.DeliveryTimeSlot);

                    _stockLockService.ReleaseReservedStock(detail.ProductId, detail.Quantity);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
