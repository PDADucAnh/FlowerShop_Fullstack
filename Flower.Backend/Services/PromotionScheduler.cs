using Flower.Backend.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
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
