using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class OrderExpiryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderExpiryBackgroundService> _logger;

        public OrderExpiryBackgroundService(IServiceScopeFactory scopeFactory, ILogger<OrderExpiryBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderExpiryBackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogDebug("Scanning for expired orders...");
                    await ProcessExpiredOrdersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing OrderExpiryBackgroundService.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("OrderExpiryBackgroundService is stopping.");
        }

        private async Task ProcessExpiredOrdersAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                var now = DateTime.UtcNow;
                var codCutoff = now.AddMinutes(-30);
                var onlineCutoff = now.AddMinutes(-15);
                var pendingCutoff = now.AddMinutes(-15);

                var expiredOrders = await context.Orders
                    .Include(o => o.OrderDetails)
                    .Where(o =>
                        (o.PaymentMethod == PaymentMethod.COD && o.Status == OrderStatus.PendingVerification && o.OrderDate <= codCutoff) ||
                        (o.PaymentMethod == PaymentMethod.OnlinePayment && o.Status == OrderStatus.PendingPayment && o.OrderDate <= onlineCutoff) ||
                        (o.PaymentMethod == PaymentMethod.OnlinePayment && o.Status == OrderStatus.Pending && o.OrderDate <= pendingCutoff)
                    )
                    .ToListAsync(stoppingToken);

                if (expiredOrders.Any())
                {
                    _logger.LogInformation("Found {Count} expired orders to cancel.", expiredOrders.Count);

                    foreach (var order in expiredOrders)
                    {
                        string reason;
                        if (order.PaymentMethod == PaymentMethod.COD)
                        {
                            reason = "Tự động hủy đơn COD quá hạn 30 phút chưa xác minh";
                        }
                        else if (order.Status == OrderStatus.PendingPayment)
                        {
                            reason = "Tự động hủy đơn hàng quá hạn thanh toán 15 phút";
                        }
                        else
                        {
                            reason = "Tự động hủy đơn hàng quá hạn thanh toán 15 phút";
                        }

                        if (order.PaymentMethod == PaymentMethod.OnlinePayment)
                        {
                            var pendingPayments = await context.Payments
                                .Where(p => p.OrderId == order.Id && p.Status == PaymentStatus.Pending)
                                .ToListAsync(stoppingToken);

                            foreach (var payment in pendingPayments)
                            {
                                payment.Status = PaymentStatus.Expired;
                            }

                            if (order.OrderDetails != null)
                            {
                                foreach (var detail in order.OrderDetails)
                                {
                                    var product = await context.Products
                                        .FirstOrDefaultAsync(p => p.Id == detail.ProductId, stoppingToken);
                                    if (product != null)
                                    {
                                        var newStock = product.StockQuantity + detail.Quantity;
                                        await context.Database.ExecuteSqlRawAsync(
                                            "UPDATE Products SET StockQuantity = {0} WHERE Id = {1}",
                                            newStock, detail.ProductId);
                                    }
                                }
                            }
                        }

                        await orderService.CancelWithReason(order.Id, reason);
                    }
                    _logger.LogInformation("Successfully cancelled {Count} expired orders centrally.", expiredOrders.Count);
                }
            }
        }
    }
}
