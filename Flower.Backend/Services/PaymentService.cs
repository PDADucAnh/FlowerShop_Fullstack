using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Models.DTOs;
using Flower.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly string _webhookSecret;

        public PaymentService(
            IApplicationDbContext context, 
            IOrderCancellationService orderCancellationService,
            StockLockService stockLockService,
            IDeliverySlotService deliverySlotService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _context = context;
            _orderCancellationService = orderCancellationService;
            _stockLockService = stockLockService;
            _deliverySlotService = deliverySlotService;
            _emailService = emailService;
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
                PaidAt = DateTime.Now
            };

            _context.Payments.Add(payment);

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.PaymentStatus = PaymentStatus.Completed;
                order.PaymentTransactionId = transactionId;
                order.PaymentPaidAt = DateTime.Now;
                order.Status = OrderStatus.Confirmed;
            }

            await _context.SaveChangesAsync();
            return payment.ToDTO();
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

                await RecordPayment(request.OrderId, request.Amount, PaymentMethod.OnlinePayment, request.TransactionId);

                if (orderWithDetails.Customer != null && !string.IsNullOrEmpty(orderWithDetails.Customer.Email))
                {
                    orderWithDetails.Status = OrderStatus.Confirmed;
                    orderWithDetails.PaymentStatus = PaymentStatus.Completed;
                    orderWithDetails.PaymentTransactionId = request.TransactionId;
                    orderWithDetails.PaymentPaidAt = DateTime.Now;
                    await _emailService.SendOrderConfirmedEmailAsync(orderWithDetails, orderWithDetails.Customer.Email, orderWithDetails.Customer.FullName);
                }

                return true;
            }

            if (request.Status == "failed" || request.Status == "cancelled")
            {
                orderWithDetails.PaymentStatus = PaymentStatus.Failed;
                await _context.SaveChangesAsync();

                // OrderCancellationService handles both cache/lock releasing and slot releasing centrally
                await _orderCancellationService.CancelWithReason(request.OrderId, "Thanh toán thất bại");

                return true;
            }

            return false;
        }

        public async Task<bool> RefundPayment(int orderId, decimal amount)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Status == PaymentStatus.Completed);

            if (payment == null) return false;

            payment.Status = amount >= payment.Amount ? PaymentStatus.Refunded : PaymentStatus.PartialRefund;
            payment.RefundedAt = DateTime.Now;

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.RefundAmount = amount;
                order.PaymentStatus = amount >= payment.Amount ? PaymentStatus.Refunded : PaymentStatus.PartialRefund;
            }

            await _context.SaveChangesAsync();
            return true;
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
