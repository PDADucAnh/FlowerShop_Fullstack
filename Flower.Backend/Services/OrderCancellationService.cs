using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class OrderCancellationService : IOrderCancellationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IDeliverySlotService _deliverySlotService;
        private readonly StockLockService _stockLockService;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderCancellationService> _logger;
        private readonly ICouponService _couponService;
        private readonly INotificationService _notificationService;

        public OrderCancellationService(
            IApplicationDbContext context,
            IDeliverySlotService deliverySlotService,
            StockLockService stockLockService,
            IEmailService emailService,
            ILogger<OrderCancellationService> logger,
            ICouponService couponService,
            INotificationService notificationService)
        {
            _context = context;
            _deliverySlotService = deliverySlotService;
            _stockLockService = stockLockService;
            _emailService = emailService;
            _logger = logger;
            _couponService = couponService;
            _notificationService = notificationService;
        }

        public async Task<bool> CancelWithReason(int id, string? reason)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                _logger.LogWarning("CancelWithReason: Order {OrderId} not found", id);
                return false;
            }

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.CancelledByCustomer || order.Status == OrderStatus.CancelledByShop)
            {
                _logger.LogInformation("CancelWithReason: Order {OrderId} already cancelled", id);
                return true;
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = reason;

            await ReleaseOrderResources(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> CancelByShop(int id, string? reason)
        {
            return await CancelWithPolicy(id, reason, "Shop");
        }

        public async Task<(bool Success, string Message)> CancelByCustomer(int id, string? reason)
        {
            return await CancelWithPolicy(id, reason, "Customer");
        }

        public async Task<(bool Success, string Message)> CancelWithPolicy(int id, string? reason, string cancelledBy)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return (false, "Đơn hàng không tồn tại");

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.CancelledByCustomer || order.Status == OrderStatus.CancelledByShop || order.Status == OrderStatus.Completed || order.Status == OrderStatus.Refunded)
                return (false, "Đơn hàng đã được xử lý trước đó");

            if (order.Status == OrderStatus.Shipping)
                return (false, "Đơn hàng đang được giao, không thể hủy. Vui lòng liên hệ hotline để được hỗ trợ.");

            if (order.Status == OrderStatus.ReadyForDelivery && cancelledBy == "Customer")
                return (false, "Đơn hàng đã sẵn sàng giao, không thể hủy. Vui lòng liên hệ hotline để được hỗ trợ.");

            var totalAmount = order.OrderDetails?.Sum(od => od.Quantity * od.UnitPrice) ?? 0;

            int refundPercent;
            int cancellationFeePercent;
            string message;

            if (cancelledBy == "Shop")
            {
                refundPercent = 100;
                cancellationFeePercent = 0;
                message = "Đơn hàng đã bị hủy bởi cửa hàng. Tiền sẽ được hoàn lại 100%.";
            }
            else
            {
                var policy = await GetPolicyForOrderStatus(order.Status);
                if (policy != null)
                {
                    refundPercent = policy.RefundPercent;
                    cancellationFeePercent = policy.CancellationFeePercent;
                }
                else
                {
                    (refundPercent, cancellationFeePercent) = order.Status switch
                    {
                        OrderStatus.Pending or OrderStatus.PendingVerification => (100, 0),
                        OrderStatus.PendingPayment or OrderStatus.Paid => (100, 0),
                        OrderStatus.Preparing => (70, 30),
                        OrderStatus.ReadyForDelivery => (50, 50),
                        _ => (100, 0)
                    };
                }

                message = cancellationFeePercent > 0
                    ? $"Hủy đơn thành công. Phí hủy {cancellationFeePercent}% giá trị đơn hàng sẽ được khấu trừ."
                    : "Hủy đơn thành công. Tiền sẽ được hoàn lại 100%.";
            }

            bool isPaid = order.PaymentMethod == PaymentMethod.OnlinePayment && order.PaymentStatus == PaymentStatus.Completed;
            var refundAmount = isPaid ? totalAmount * refundPercent / 100 : 0;
            var cancellationFee = totalAmount * cancellationFeePercent / 100;

            order.Status = cancelledBy == "Shop" ? OrderStatus.CancelledByShop : OrderStatus.CancelledByCustomer;
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = reason ?? (cancelledBy == "Shop" ? "Cửa hàng hủy đơn" : "Khách hàng hủy đơn");
            order.CancelledBy = cancelledBy;
            order.CancellationFee = cancellationFee;
            order.RefundAmount = refundAmount;
            order.RefundRequestedAt = DateTime.UtcNow;

            if (refundAmount > 0)
            {
                order.PaymentStatus = refundPercent >= 100 ? PaymentStatus.RefundPending : PaymentStatus.PartialRefundPending;
                order.Status = OrderStatus.RefundPending;
            }

            await ReleaseOrderResources(order);

            var refund = new Refund
            {
                OrderId = order.Id,
                RequestedBy = cancelledBy,
                Reason = order.CancellationReason,
                RefundType = cancelledBy == "Shop" ? "Admin" : "Customer",
                RefundPercent = refundPercent,
                RefundAmount = refundAmount,
                RefundStatus = refundAmount > 0 ? 0 : 2,
                ProcessedAt = refundAmount > 0 ? null : DateTime.UtcNow
            };
            _context.Refunds.Add(refund);

            await _context.SaveChangesAsync();

            if (order.Customer != null && !string.IsNullOrEmpty(order.Customer.Email))
            {
                try
                {
                    if (cancelledBy == "Shop")
                    {
                        await _emailService.SendOrderCancelledByShopEmailAsync(order, order.Customer.Email, order.Customer.FullName, reason ?? "Cửa hàng hủy đơn", refundAmount);
                    }
                    else if (cancellationFeePercent > 0)
                    {
                        await _emailService.SendOrderCancelledWithFeeEmailAsync(order, order.Customer.Email, order.Customer.FullName, refundAmount, cancellationFeePercent, cancellationFee);
                    }
                    else
                    {
                        await _emailService.SendOrderCancelledByCustomerEmailAsync(order, order.Customer.Email, order.Customer.FullName, refundAmount);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send cancellation email for order {OrderId}", order.Id);
                }
            }

            try
            {
                var notifTitle = cancelledBy == "Shop"
                    ? $"Đơn hàng #{order.Id} đã bị hủy bởi cửa hàng"
                    : $"Đơn hàng #{order.Id} đã được hủy theo yêu cầu";
                var notifContent = refundAmount > 0
                    ? $"Số tiền hoàn: {refundAmount:N0} VNĐ. Tiền sẽ được hoàn trong vòng 24 giờ."
                    : "Đơn hàng đã được hủy.";

                await _notificationService.CreateCustomerNotification(
                    customerId: order.CustomerId,
                    title: notifTitle,
                    content: notifContent,
                    type: "OrderCancelled",
                    orderId: order.Id,
                    referenceType: "OrderCancelled",
                    icon: cancelledBy == "Shop" ? "CancelScheduleSend" : "Cancel",
                    priority: "High",
                    navigationUrl: $"/my-orders/{order.Id}"
                );

                if (order.CustomerId > 0)
                {
                    await _notificationService.NotifyCustomerEvent(order.CustomerId, "OrderChanged", new { orderId = order.Id, status = order.Status.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send notification for order {OrderId}", order.Id);
            }

            if (refundAmount > 0)
                message += $" Số tiền hoàn: {refundAmount:N0} VNĐ.";

            return (true, message);
        }

        public async Task<CancellationPolicy?> GetPolicyForOrderStatus(OrderStatus status)
        {
            var statusName = status.ToString();
            return await _context.CancellationPolicies
                .FirstOrDefaultAsync(cp => cp.OrderStatus == statusName && cp.IsActive);
        }

        public async Task SeedDefaultPoliciesAsync()
        {
            if (await _context.CancellationPolicies.AnyAsync())
                return;

            var policies = new List<CancellationPolicy>
            {
                new() { OrderStatus = nameof(OrderStatus.Pending), RefundPercent = 100, CancellationFeePercent = 0, Description = "Chưa xử lý", IsActive = true },
                new() { OrderStatus = nameof(OrderStatus.PendingVerification), RefundPercent = 100, CancellationFeePercent = 0, Description = "Chờ xác nhận", IsActive = true },
                new() { OrderStatus = nameof(OrderStatus.PendingPayment), RefundPercent = 100, CancellationFeePercent = 0, Description = "Chờ thanh toán", IsActive = true },
                new() { OrderStatus = nameof(OrderStatus.Paid), RefundPercent = 100, CancellationFeePercent = 0, Description = "Đã thanh toán - chưa chuẩn bị", IsActive = true },
                new() { OrderStatus = nameof(OrderStatus.Confirmed), RefundPercent = 80, CancellationFeePercent = 20, Description = "Đã xác nhận - đã chuẩn bị nguyên liệu", IsActive = true },
                new() { OrderStatus = nameof(OrderStatus.Preparing), RefundPercent = 70, CancellationFeePercent = 30, Description = "Đang cắm hoa", IsActive = true },
                new() { OrderStatus = nameof(OrderStatus.ReadyForDelivery), RefundPercent = 50, CancellationFeePercent = 50, Description = "Đã hoàn thành - sẵn sàng giao", IsActive = true },
            };

            _context.CancellationPolicies.AddRange(policies);
            await _context.SaveChangesAsync();
        }

        private async Task ReleaseOrderResources(Order order)
        {
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

            await _couponService.ReleaseCoupon(order.Id);
        }
    }
}
