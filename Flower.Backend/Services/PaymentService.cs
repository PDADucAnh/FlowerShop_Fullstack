using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IOrderCancellationService _orderCancellationService;
        private readonly StockLockService _stockLockService;
        private readonly IDeliverySlotService _deliverySlotService;
        private readonly IEmailService _emailService;
        private readonly ILogger<PaymentService> _logger;
        private readonly INotificationService _notificationService;
        private readonly string _webhookSecret;

        public PaymentService(
            IApplicationDbContext context, 
            IOrderCancellationService orderCancellationService,
            StockLockService stockLockService,
            IDeliverySlotService deliverySlotService,
            IEmailService emailService,
            ILogger<PaymentService> logger,
            INotificationService notificationService,
            IConfiguration configuration)
        {
            _context = context;
            _orderCancellationService = orderCancellationService;
            _stockLockService = stockLockService;
            _deliverySlotService = deliverySlotService;
            _emailService = emailService;
            _logger = logger;
            _notificationService = notificationService;
            _webhookSecret = configuration["WebhookSettings:SecretKey"] ?? "flowershop-webhook-secret-change-in-production";
        }

        public async Task<PaymentDTO> RecordPayment(int orderId, decimal amount, PaymentMethod method, string? transactionId = null)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = method,
                Status = PaymentStatus.Completed,
                TransactionId = transactionId,
                PaidAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.PaymentStatus = PaymentStatus.Completed;
                order.Status = OrderStatus.Paid;
                
                if (order.CustomerId > 0)
                {
                    await _notificationService.NotifyCustomerEvent(order.CustomerId, "OrderChanged", new { orderId = order.Id, paymentStatus = "Completed" });
                    await _notificationService.CreateCustomerNotification(
                        customerId: order.CustomerId,
                        title: $"Thanh toán đơn hàng #{order.Id} thành công",
                        content: $"Đơn hàng #{order.Id} đã được thanh toán {amount:N0} VNĐ.",
                        type: "PaymentCompleted",
                        orderId: order.Id,
                        referenceType: "PaymentCompleted",
                        icon: "Payment",
                        priority: "High",
                        navigationUrl: $"/my-orders/{order.Id}"
                    );
                }
                order.PaymentTransactionId = transactionId;
                order.PaymentPaidAt = DateTime.UtcNow;
                order.Status = OrderStatus.Confirmed;
                order.IsVerified = true;
                order.VerifiedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return payment.ToDTO();
        }

        public async Task<int> CreatePendingPayment(int orderId, decimal amount, PaymentMethod method)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = method,
                Status = PaymentStatus.Pending
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created Pending Payment: PaymentId={PaymentId}, OrderId={OrderId}, Amount={Amount}",
                payment.Id, orderId, amount);

            return payment.Id;
        }

        public async Task<bool> ProcessWebhook(PaymentWebhookRequest request)
        {
            if (!VerifyWebhookSignature(request))
                return false;

            var orderWithDetails = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (orderWithDetails == null) return false;

            // Idempotency check: if order is already paid, do not process again
            if (orderWithDetails.PaymentStatus == PaymentStatus.Completed) return true;

            if (request.Status == "success" || request.Status == "completed")
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    if (orderWithDetails.OrderDetails != null)
                    {
                        var productIds = orderWithDetails.OrderDetails.Select(od => od.ProductId).ToList();
                        var products = await _context.Products
                            .Where(p => productIds.Contains(p.Id))
                            .ToListAsync();

                        foreach (var item in orderWithDetails.OrderDetails)
                        {
                            var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                            if (product != null)
                            {
                                product.StockQuantity -= item.Quantity;
                            }

                            _stockLockService.ReleaseReservedStock(item.ProductId, item.Quantity);
                        }
                    }

                    orderWithDetails.Status = OrderStatus.Confirmed;
                    orderWithDetails.IsVerified = true;
                    orderWithDetails.VerifiedAt = DateTime.UtcNow;
                    orderWithDetails.PaymentTransactionId = request.TransactionId;
                    orderWithDetails.PaymentPaidAt = DateTime.UtcNow;

                    await RecordPayment(request.OrderId, request.Amount, PaymentMethod.OnlinePayment, request.TransactionId);

                    await transaction.CommitAsync();

                    if (orderWithDetails.Customer != null && !string.IsNullOrEmpty(orderWithDetails.Customer.Email))
                    {
                        await _emailService.SendOrderConfirmedEmailAsync(orderWithDetails, orderWithDetails.Customer.Email, orderWithDetails.Customer.FullName);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Webhook processing failed for order {OrderId}", request.OrderId);
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            if (request.Status == "failed" || request.Status == "cancelled")
            {
                orderWithDetails.PaymentStatus = PaymentStatus.Failed;
                if (orderWithDetails.CustomerId > 0)
                {
                    await _notificationService.NotifyCustomerEvent(orderWithDetails.CustomerId, "OrderChanged", new { orderId = orderWithDetails.Id, paymentStatus = "Failed" });
                    await _notificationService.CreateCustomerNotification(
                        customerId: orderWithDetails.CustomerId,
                        title: $"Thanh toán đơn hàng #{orderWithDetails.Id} thất bại",
                        content: "Giao dịch thanh toán không thành công. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.",
                        type: "PaymentFailed",
                        orderId: orderWithDetails.Id,
                        referenceType: "PaymentFailed",
                        icon: "PaymentError",
                        priority: "High",
                        navigationUrl: $"/my-orders/{orderWithDetails.Id}"
                    );
                }
                await _context.SaveChangesAsync();

                // OrderCancellationService handles both cache/lock releasing and slot releasing centrally
                await _orderCancellationService.CancelWithReason(request.OrderId, "Thanh toán thất bại");

                return true;
            }

            return false;
        }

        public async Task<(bool Success, string Message)> ConfirmOnlinePayment(int orderId, string transactionId, decimal amount, string? ipAddress = null, string? userAgent = null, string? gatewayResponse = null)
        {
            var orderWithDetails = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (orderWithDetails == null)
            {
                _logger.LogWarning("ConfirmOnlinePayment: order not found, OrderId={OrderId}", orderId);
                return (false, "Đơn hàng không tồn tại");
            }

            if (orderWithDetails.PaymentStatus == PaymentStatus.Completed)
            {
                _logger.LogInformation("ConfirmOnlinePayment: order already paid, OrderId={OrderId}", orderId);
                return (true, "Đơn hàng đã được thanh toán trước đó");
            }

            if (orderWithDetails.PaymentMethod != PaymentMethod.OnlinePayment)
            {
                _logger.LogWarning("ConfirmOnlinePayment: not an online payment order, OrderId={OrderId}", orderId);
                return (false, "Phương thức thanh toán không hợp lệ");
            }

            var pendingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status == PaymentStatus.Pending);

            if (pendingPayment == null)
            {
                _logger.LogWarning("ConfirmOnlinePayment: no pending payment found, OrderId={OrderId}", orderId);
                return (false, "Không tìm thấy yêu cầu thanh toán");
            }

            if (orderWithDetails.FinalAmount != amount)
            {
                _logger.LogWarning("ConfirmOnlinePayment: amount mismatch, OrderId={OrderId}, FinalAmount={FinalAmount}, Got={Got}",
                    orderId, orderWithDetails.FinalAmount, amount);
                return (false, "Số tiền thanh toán không khớp");
            }

            _logger.LogInformation("ConfirmOnlinePayment: starting, OrderId={OrderId}, TransactionId={TransactionId}, Amount={Amount}",
                orderId, transactionId, amount);

            var attemptCount = await _context.PaymentAttempts
                .CountAsync(pa => pa.PaymentId == pendingPayment.Id);

            await using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                pendingPayment.Status = PaymentStatus.Completed;
                pendingPayment.TransactionId = transactionId;
                pendingPayment.PaidAt = DateTime.UtcNow;

                orderWithDetails.PaymentStatus = PaymentStatus.Completed;
                orderWithDetails.Status = OrderStatus.Confirmed;
                
                if (orderWithDetails.CustomerId > 0)
                {
                    await _notificationService.NotifyCustomerEvent(orderWithDetails.CustomerId, "OrderChanged", new { orderId = orderWithDetails.Id, paymentStatus = "Completed" });
                    await _notificationService.CreateCustomerNotification(
                        customerId: orderWithDetails.CustomerId,
                        title: $"Thanh toán đơn hàng #{orderWithDetails.Id} thành công",
                        content: $"Đơn hàng #{orderWithDetails.Id} đã được thanh toán {amount:N0} VNĐ.",
                        type: "PaymentCompleted",
                        orderId: orderWithDetails.Id,
                        referenceType: "PaymentCompleted",
                        icon: "Payment",
                        priority: "High",
                        navigationUrl: $"/my-orders/{orderWithDetails.Id}"
                    );
                }

                orderWithDetails.IsVerified = true;
                orderWithDetails.VerifiedAt = DateTime.UtcNow;
                orderWithDetails.PaymentTransactionId = transactionId;
                orderWithDetails.PaymentPaidAt = DateTime.UtcNow;

                _context.PaymentAttempts.Add(new PaymentAttempt
                {
                    PaymentId = pendingPayment.Id,
                    AttemptNumber = attemptCount + 1,
                    GatewayResponse = gatewayResponse ?? transactionId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                });

                if (orderWithDetails.OrderDetails != null)
                {
                    var productIds = orderWithDetails.OrderDetails.Select(od => od.ProductId).ToList();
                    var products = await _context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToListAsync();

                    foreach (var item in orderWithDetails.OrderDetails)
                    {
                        var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product != null)
                            product.StockQuantity -= item.Quantity;

                        _stockLockService.ReleaseReservedStock(item.ProductId, item.Quantity);
                    }
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                _logger.LogInformation("Payment success: OrderId={OrderId}, TransactionId={TransactionId}, Amount={Amount}",
                    orderId, transactionId, amount);

                if (orderWithDetails.Customer != null && !string.IsNullOrEmpty(orderWithDetails.Customer.Email))
                {
                    try
                    {
                        await _emailService.SendOrderConfirmedEmailAsync(orderWithDetails, orderWithDetails.Customer.Email, orderWithDetails.Customer.FullName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to send confirmation email for order {OrderId}", orderId);
                    }
                }

                return (true, "Thanh toán thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ConfirmOnlinePayment failed for order {OrderId}", orderId);
                await dbTransaction.RollbackAsync();
                return (false, "Lỗi hệ thống khi xử lý thanh toán");
            }
        }

        public async Task<bool> RefundPayment(int orderId, decimal amount)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status == PaymentStatus.Completed);

            if (payment == null) return false;

            payment.Status = amount >= payment.Amount ? PaymentStatus.Refunded : PaymentStatus.PartialRefunded;
            payment.RefundedAt = DateTime.UtcNow;

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.RefundAmount = amount;
                order.PaymentStatus = amount >= payment.Amount ? PaymentStatus.Refunded : PaymentStatus.PartialRefunded;
                order.Status = OrderStatus.Refunded;
                order.RefundCompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> ProcessRefund(int orderId, string? transactionId = null, string? responseCode = null, string? processedBy = null)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return (false, "Đơn hàng không tồn tại");

            if (order.PaymentStatus != PaymentStatus.RefundPending && order.PaymentStatus != PaymentStatus.PartialRefundPending)
                return (false, "Đơn hàng không ở trạng thái chờ hoàn tiền");

            if (order.RefundAmount <= 0)
                return (false, "Số tiền hoàn không hợp lệ");

            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId && (p.Status == PaymentStatus.Completed || p.Status == PaymentStatus.RefundPending || p.Status == PaymentStatus.PartialRefundPending));

            if (payment != null)
            {
                payment.Status = order.RefundAmount >= payment.Amount ? PaymentStatus.Refunded : PaymentStatus.PartialRefunded;
                payment.RefundedAt = DateTime.UtcNow;
                payment.RefundTransactionId = transactionId;
                payment.RefundResponseCode = responseCode;
                payment.RefundedBy = processedBy;
            }

            order.PaymentStatus = order.RefundAmount >= (payment?.Amount ?? 0) ? PaymentStatus.Refunded : PaymentStatus.PartialRefunded;
            order.Status = OrderStatus.Refunded;
            order.RefundCompletedAt = DateTime.UtcNow;

            var refund = await _context.Refunds
                .FirstOrDefaultAsync(r => r.OrderId == orderId && r.RefundStatus == 0);
            if (refund != null)
            {
                refund.RefundStatus = 1;
                refund.ApprovedBy = processedBy;
                refund.GatewayRefundId = transactionId;
                refund.ProcessedAt = DateTime.UtcNow;
                refund.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            if (order.Customer != null && !string.IsNullOrEmpty(order.Customer.Email))
            {
                try
                {
                    var paymentMethod = order.PaymentMethod == PaymentMethod.COD ? "COD" : "VNPay";
                    await _emailService.SendRefundCompletedEmailAsync(order, order.Customer.Email, order.Customer.FullName, order.RefundAmount, paymentMethod, transactionId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send refund email for order {OrderId}", orderId);
                }
            }

            try
            {
                if (order.CustomerId > 0)
                {
                    await _notificationService.CreateCustomerNotification(
                        customerId: order.CustomerId,
                        title: $"Hoàn tiền đơn hàng #{order.Id} thành công",
                        content: $"Số tiền đã hoàn: {order.RefundAmount:N0} VNĐ.",
                        type: "RefundCompleted",
                        orderId: order.Id,
                        referenceType: "RefundCompleted",
                        icon: "MoneyOff",
                        priority: "High",
                        navigationUrl: $"/my-orders/{order.Id}"
                    );

                    await _notificationService.NotifyCustomerEvent(order.CustomerId, "OrderChanged", new { orderId = order.Id, status = order.Status.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create refund notification for order {OrderId}", orderId);
            }

            return (true, "Hoàn tiền thành công");
        }

        public async Task<(bool Success, string Message)> MarkPaymentFailed(int orderId, string? gatewayResponse = null, string? ipAddress = null, string? userAgent = null)
        {
            var pendingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status == PaymentStatus.Pending);

            if (pendingPayment == null)
            {
                _logger.LogWarning("MarkPaymentFailed: no pending payment found, OrderId={OrderId}", orderId);
                return (false, "Không tìm thấy yêu cầu thanh toán");
            }

            pendingPayment.Status = PaymentStatus.Failed;
            pendingPayment.GatewayResponseCode = gatewayResponse;

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.PaymentStatus = PaymentStatus.Failed;
            }

            var attemptCount = await _context.PaymentAttempts
                .CountAsync(pa => pa.PaymentId == pendingPayment.Id);

            _context.PaymentAttempts.Add(new PaymentAttempt
            {
                PaymentId = pendingPayment.Id,
                AttemptNumber = attemptCount + 1,
                GatewayResponse = gatewayResponse,
                IpAddress = ipAddress,
                UserAgent = userAgent
            });

            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment marked as failed: OrderId={OrderId}, PaymentId={PaymentId}, ResponseCode={Code}",
                orderId, pendingPayment.Id, gatewayResponse);

            return (true, "Cập nhật trạng thái thanh toán thất bại");
        }

        public async Task<(bool Success, string Message, int PaymentId, string? PaymentUrl)> CreateRetryPayment(int orderId, PaymentMethod method)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return (false, "Đơn hàng không tồn tại", 0, null);

            if (order.PaymentMethod != PaymentMethod.OnlinePayment)
                return (false, "Phương thức thanh toán không hợp lệ", 0, null);

            if (order.Status == OrderStatus.Paid || order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                return (false, "Đơn hàng không thể thanh toán lại", 0, null);

            if (order.Status != OrderStatus.PendingPayment)
                return (false, "Đơn hàng không ở trạng thái chờ thanh toán", 0, null);

            var totalAmount = order.FinalAmount;
            if (totalAmount <= 0)
                return (false, "Số tiền thanh toán không hợp lệ", 0, null);

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = totalAmount,
                Method = method,
                Status = PaymentStatus.Pending
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created retry Payment: PaymentId={PaymentId}, OrderId={OrderId}, Amount={Amount}",
                payment.Id, orderId, totalAmount);

            return (true, "Tạo yêu cầu thanh toán thành công", payment.Id, null);
        }

        public async Task<PaymentDTO?> GetByOrderId(int orderId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
            return payment?.ToDTO();
        }

        private bool VerifyWebhookSignature(PaymentWebhookRequest request)
        {
            if (string.IsNullOrEmpty(request.Signature))
                return false;

            var payload = $"{request.TransactionId}|{request.OrderId}|{request.Amount}|{request.Status}";
            var computedHash = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(_webhookSecret),
                Encoding.UTF8.GetBytes(payload));
            var computedSignature = Convert.ToBase64String(computedHash);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedSignature),
                Encoding.UTF8.GetBytes(request.Signature));
        }
    }
}
