using Flower.Backend.Services.Interfaces;
using Flower.Data;
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
    public class PromotionScheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PromotionScheduler> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public PromotionScheduler(IServiceProvider serviceProvider, ILogger<PromotionScheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PromotionScheduler started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var promotionService = scope.ServiceProvider.GetRequiredService<IPromotionService>();
                    await promotionService.AutoActivateExpired();

                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var adminNotificationService = scope.ServiceProvider.GetRequiredService<IAdminNotificationService>();

                    var utcNow = DateTime.UtcNow;
                    var threshold24h = utcNow.AddHours(24);

                    // 1. Check expiring Flash Sales
                    var expiringFlashSales = await db.FlashSales
                        .Where(fs => fs.IsActive && fs.EndDate > utcNow && fs.EndDate <= threshold24h)
                        .ToListAsync(stoppingToken);

                    foreach (var fs in expiringFlashSales)
                    {
                        var refId = $"FlashSale_Expiring_{fs.Id}";
                        var alreadyNotified = await db.AdminNotifications
                            .AnyAsync(n => n.Type == "Promotion" && n.ReferenceId == refId, stoppingToken);

                        if (!alreadyNotified)
                        {
                            var timeLeft = fs.EndDate - utcNow;
                            var hoursLeft = Math.Round(timeLeft.TotalHours, 1);
                            await adminNotificationService.CreateNotification(
                                "Flash Sale sắp hết hạn",
                                $"Đợt Flash Sale '{fs.Name}' sắp hết hạn (còn {hoursLeft} giờ, kết thúc lúc {fs.EndDate.AddHours(7):dd/MM/yyyy HH:mm}).",
                                "Promotion",
                                refId
                            );
                        }
                    }

                    // 2. Check expiring Coupons
                    var expiringCoupons = await db.Coupons
                        .Where(c => c.IsActive && c.EndDate.HasValue && c.EndDate.Value > utcNow && c.EndDate.Value <= threshold24h)
                        .ToListAsync(stoppingToken);

                    foreach (var c in expiringCoupons)
                    {
                        var refId = $"Coupon_Expiring_{c.Id}";
                        var alreadyNotified = await db.AdminNotifications
                            .AnyAsync(n => n.Type == "Promotion" && n.ReferenceId == refId, stoppingToken);

                        if (!alreadyNotified)
                        {
                            var timeLeft = c.EndDate.Value - utcNow;
                            var hoursLeft = Math.Round(timeLeft.TotalHours, 1);
                            await adminNotificationService.CreateNotification(
                                "Coupon sắp hết hạn",
                                $"Mã giảm giá '{c.Code}' sắp hết hạn (còn {hoursLeft} giờ, kết thúc lúc {c.EndDate.Value.AddHours(7):dd/MM/yyyy HH:mm}).",
                                "Promotion",
                                refId
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PromotionScheduler execution");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
