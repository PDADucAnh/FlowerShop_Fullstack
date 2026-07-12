using Flower.Data;
using Flower.Data.Entities;
using Flower.Backend.Services.Interfaces;
using Flower.Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DashboardService> _logger;
        private readonly IPromotionService _promotionService;

        public DashboardService(IApplicationDbContext context, ILogger<DashboardService> logger, IPromotionService promotionService)
        {
            _context = context;
            _logger = logger;
            _promotionService = promotionService;
        }

        public async Task<DashboardSummaryDTO> GetSummary()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = todayStart.AddDays(-(int)now.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var revenueRows = await (
                from o in _context.Orders
                join od in _context.OrderDetails on o.Id equals od.OrderId
                where o.Status == OrderStatus.Completed && o.PaymentStatus == PaymentStatus.Completed
                select new { o.OrderDate, od.UnitPrice, od.Quantity }
            ).ToListAsync();

            var revenue = new DashboardRevenueDTO
            {
                Today = revenueRows.Where(r => r.OrderDate >= todayStart).Sum(r => r.UnitPrice * r.Quantity),
                Week = revenueRows.Where(r => r.OrderDate >= weekStart).Sum(r => r.UnitPrice * r.Quantity),
                Month = revenueRows.Where(r => r.OrderDate >= monthStart).Sum(r => r.UnitPrice * r.Quantity),
                Year = revenueRows.Where(r => r.OrderDate >= yearStart).Sum(r => r.UnitPrice * r.Quantity),
            };

            var ordersQuery = _context.Orders;
            var orders = new DashboardOrderDTO
            {
                New = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Pending),
                PendingConfirmation = await ordersQuery.CountAsync(o => o.Status == OrderStatus.PendingVerification),
                Preparing = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Preparing),
                Arranging = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Confirmed),
                ReadyForDelivery = await ordersQuery.CountAsync(o => o.Status == OrderStatus.ReadyForDelivery),
                Delivering = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Shipping),
                Completed = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Completed),
                Cancelled = await ordersQuery.CountAsync(o =>
                    o.Status == OrderStatus.Cancelled ||
                    o.Status == OrderStatus.CancelledByCustomer ||
                    o.Status == OrderStatus.CancelledByShop),
            };

            var customersQuery = _context.Customers;
            var thirtyDaysAgo = now.AddDays(-30);
            var customers = new DashboardCustomerDTO
            {
                Total = await customersQuery.CountAsync(),
                New = await customersQuery.CountAsync(c => c.CreatedAt >= monthStart),
                Active = await customersQuery.CountAsync(c => c.LastLogin >= thirtyDaysAgo),
                Locked = await customersQuery.CountAsync(c => !c.IsActive),
            };

            var productsQuery = _context.Products;
            var products = new DashboardProductStatsDTO
            {
                Total = await productsQuery.CountAsync(),
                Active = await productsQuery.CountAsync(p => p.IsActive),
                OutOfStock = await productsQuery.CountAsync(p => p.StockQuantity <= 0),
                Discontinued = await productsQuery.CountAsync(p => !p.IsActive),
            };

            var paymentsQuery = _context.Payments;
            var payments = new DashboardPaymentStatsDTO
            {
                VnPay = await paymentsQuery.CountAsync(p => p.Gateway == "VNPAY"),
                Transfer = await paymentsQuery.CountAsync(p => p.Gateway == "BANK_TRANSFER"),
                Cash = await paymentsQuery.CountAsync(p => p.Method == PaymentMethod.COD),
                Pending = await paymentsQuery.CountAsync(p => p.Status == PaymentStatus.Pending),
                Failed = await paymentsQuery.CountAsync(p => p.Status == PaymentStatus.Failed),
                Refunded = await paymentsQuery.CountAsync(p =>
                    p.Status == PaymentStatus.Refunded ||
                    p.Status == PaymentStatus.PartialRefunded),
            };

            var lowStockThreshold = 10;
            var inventory = new DashboardInventoryDTO
            {
                InStock = await productsQuery.CountAsync(p => p.StockQuantity > lowStockThreshold),
                LowStock = await productsQuery.CountAsync(p =>
                    p.StockQuantity > 0 && p.StockQuantity <= lowStockThreshold),
                OutOfStock = await productsQuery.CountAsync(p => p.StockQuantity <= 0),
            };

            var reviews = new DashboardReviewDTO
            {
                TotalReviews = 0,
                AverageRating = 0,
                LatestReviews = 0,
            };

            var banners = new DashboardBannerDTO
            {
                Active = await _context.Advertisements.CountAsync(a => a.IsActive),
                Expired = await _context.Advertisements.CountAsync(a => !a.IsActive),
            };

            var notifications = await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .Select(n => new DashboardNotificationDTO
                {
                    Id = n.Id,
                    Title = n.Title ?? "",
                    Content = n.Content ?? "",
                    Type = n.Type ?? "",
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                })
                .ToListAsync();

            var topProducts = await _context.OrderDetails
                .GroupBy(od => new { od.ProductId, od.ProductName, od.ProductImage })
                .Select(g => new TopProductDTO
                {
                    Id = g.Key.ProductId,
                    Name = g.Key.ProductName,
                    ImageUrl = g.Key.ProductImage,
                    TotalSold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.UnitPrice * od.Quantity),
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(10)
                .ToListAsync();

            var topCustomers = await (
                from o in _context.Orders
                join od in _context.OrderDetails on o.Id equals od.OrderId
                where o.Status == OrderStatus.Completed
                group new { od.UnitPrice, od.Quantity, o.CustomerId, o.Customer!.FullName, o.Customer.Email, OrderId = o.Id }
                    by new { o.CustomerId, o.Customer!.FullName, o.Customer.Email } into g
                select new TopCustomerDTO
                {
                    Id = g.Key.CustomerId,
                    FullName = g.Key.FullName,
                    Email = g.Key.Email,
                    TotalOrders = g.Select(x => x.OrderId).Distinct().Count(),
                    TotalSpent = g.Sum(x => x.UnitPrice * x.Quantity),
                }
            ).OrderByDescending(c => c.TotalSpent).Take(10).ToListAsync();

            var activePromotions = await _promotionService.GetActivePromotions();
            var allCampaigns = await _promotionService.GetAll();

            return new DashboardSummaryDTO
            {
                Revenue = revenue,
                Orders = orders,
                Customers = customers,
                Products = products,
                Payments = payments,
                Inventory = inventory,
                Reviews = reviews,
                Banners = banners,
                Notifications = notifications,
                TopProducts = topProducts,
                TopCustomers = topCustomers,
                ActivePromotions = activePromotions?.Count() ?? 0,
                TotalDiscountGiven = 0,
                PromotionUsageCount = 0,
            };
        }

        public async Task<DashboardRevenueDTO> GetRevenue()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = todayStart.AddDays(-(int)now.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var revenueRows = await (
                from o in _context.Orders
                join od in _context.OrderDetails on o.Id equals od.OrderId
                where o.Status == OrderStatus.Completed && o.PaymentStatus == PaymentStatus.Completed
                select new { o.OrderDate, od.UnitPrice, od.Quantity }
            ).ToListAsync();

            return new DashboardRevenueDTO
            {
                Today = revenueRows.Where(r => r.OrderDate >= todayStart).Sum(r => r.UnitPrice * r.Quantity),
                Week = revenueRows.Where(r => r.OrderDate >= weekStart).Sum(r => r.UnitPrice * r.Quantity),
                Month = revenueRows.Where(r => r.OrderDate >= monthStart).Sum(r => r.UnitPrice * r.Quantity),
                Year = revenueRows.Where(r => r.OrderDate >= yearStart).Sum(r => r.UnitPrice * r.Quantity),
            };
        }

        public async Task<DashboardOrderDTO> GetOrders()
        {
            var ordersQuery = _context.Orders;
            return new DashboardOrderDTO
            {
                New = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Pending),
                PendingConfirmation = await ordersQuery.CountAsync(o => o.Status == OrderStatus.PendingVerification),
                Preparing = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Preparing),
                Arranging = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Confirmed),
                ReadyForDelivery = await ordersQuery.CountAsync(o => o.Status == OrderStatus.ReadyForDelivery),
                Delivering = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Shipping),
                Completed = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Completed),
                Cancelled = await ordersQuery.CountAsync(o =>
                    o.Status == OrderStatus.Cancelled ||
                    o.Status == OrderStatus.CancelledByCustomer ||
                    o.Status == OrderStatus.CancelledByShop),
            };
        }

        public async Task<DashboardProductStatsDTO> GetProducts()
        {
            var query = _context.Products;
            return new DashboardProductStatsDTO
            {
                Total = await query.CountAsync(),
                Active = await query.CountAsync(p => p.IsActive),
                OutOfStock = await query.CountAsync(p => p.StockQuantity <= 0),
                Discontinued = await query.CountAsync(p => !p.IsActive),
            };
        }

        public async Task<DashboardCustomerDTO> GetCustomers()
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var thirtyDaysAgo = now.AddDays(-30);

            var query = _context.Customers;
            return new DashboardCustomerDTO
            {
                Total = await query.CountAsync(),
                New = await query.CountAsync(c => c.CreatedAt >= monthStart),
                Active = await query.CountAsync(c => c.LastLogin >= thirtyDaysAgo),
                Locked = await query.CountAsync(c => !c.IsActive),
            };
        }

        public async Task<DashboardChartsDTO> GetCharts()
        {
            var now = DateTime.UtcNow;
            var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var revenueRows = await (
                from od in _context.OrderDetails
                join o in _context.Orders on od.OrderId equals o.Id
                where o.Status == OrderStatus.Completed && o.PaymentStatus == PaymentStatus.Completed
                      && o.OrderDate >= yearStart
                select new { o.OrderDate.Year, o.OrderDate.Month, od.UnitPrice, od.Quantity }
            ).ToListAsync();

            var monthlyRevenue = revenueRows
                .GroupBy(r => new { r.Year, r.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(r => r.UnitPrice * r.Quantity) })
                .ToList();

            var revenueChart = new DashboardRevenueChartDTO();
            for (int m = 1; m <= now.Month; m++)
            {
                var monthName = new DateTime(now.Year, m, 1).ToString("MMM");
                revenueChart.Labels.Add(monthName);
                var monthData = monthlyRevenue
                    .Where(r => r.Month == m)
                    .Sum(r => r.Revenue);
                revenueChart.Data.Add(monthData);
            }

            var ordersQuery = _context.Orders;
            var orderChart = new DashboardOrderChartDTO
            {
                Pending = await ordersQuery.CountAsync(o =>
                    o.Status == OrderStatus.Pending || o.Status == OrderStatus.PendingVerification),
                Preparing = await ordersQuery.CountAsync(o =>
                    o.Status == OrderStatus.Preparing || o.Status == OrderStatus.Confirmed),
                Delivering = await ordersQuery.CountAsync(o =>
                    o.Status == OrderStatus.Shipping || o.Status == OrderStatus.ReadyForDelivery),
                Completed = await ordersQuery.CountAsync(o => o.Status == OrderStatus.Completed),
                Cancelled = await ordersQuery.CountAsync(o =>
                    o.Status == OrderStatus.Cancelled ||
                    o.Status == OrderStatus.CancelledByCustomer ||
                    o.Status == OrderStatus.CancelledByShop),
            };

            var paymentsQuery = _context.Payments;
            var paymentChart = new DashboardPaymentChartDTO
            {
                VnPay = await paymentsQuery.CountAsync(p => p.Gateway == "VNPAY"),
                Transfer = await paymentsQuery.CountAsync(p => p.Gateway == "BANK_TRANSFER"),
                Cash = await paymentsQuery.CountAsync(p => p.Method == PaymentMethod.COD),
            };

            var categoryRows = await (
                from od in _context.OrderDetails
                join o in _context.Orders on od.OrderId equals o.Id
                join p in _context.Products on od.ProductId equals p.Id
                join cp in _context.CategoriesProducts on p.CategoryProductId equals cp.Id into cpJoin
                from cp in cpJoin.DefaultIfEmpty()
                where o.Status == OrderStatus.Completed && o.PaymentStatus == PaymentStatus.Completed
                select new { od.UnitPrice, od.Quantity, CategoryName = cp != null ? cp.Name : "Khác" }
            ).ToListAsync();

            var categoryRevenue = categoryRows
                .GroupBy(r => r.CategoryName)
                .Select(g => new CategoryRevenueItem
                {
                    CategoryName = g.Key,
                    Revenue = g.Sum(r => r.UnitPrice * r.Quantity),
                })
                .ToList();

            return new DashboardChartsDTO
            {
                Revenue = revenueChart,
                Orders = orderChart,
                Payments = paymentChart,
                CategoryRevenue = new DashboardCategoryRevenueDTO { Items = categoryRevenue },
            };
        }

        public async Task<List<DashboardNotificationDTO>> GetNotifications()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(20)
                .Select(n => new DashboardNotificationDTO
                {
                    Id = n.Id,
                    Title = n.Title ?? "",
                    Content = n.Content ?? "",
                    Type = n.Type ?? "",
                    CreatedAt = n.CreatedAt,
                    IsRead = n.IsRead,
                })
                .ToListAsync();
        }
    }
}
