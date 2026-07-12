using System;
using System.Collections.Generic;

namespace Flower.Backend.Models.DTOs
{
    public class DashboardSummaryDTO
    {
        public DashboardRevenueDTO Revenue { get; set; } = new();
        public DashboardOrderDTO Orders { get; set; } = new();
        public DashboardCustomerDTO Customers { get; set; } = new();
        public DashboardProductStatsDTO Products { get; set; } = new();
        public DashboardPaymentStatsDTO Payments { get; set; } = new();
        public DashboardInventoryDTO Inventory { get; set; } = new();
        public DashboardReviewDTO Reviews { get; set; } = new();
        public DashboardBannerDTO Banners { get; set; } = new();
        public List<DashboardNotificationDTO> Notifications { get; set; } = new();
        public List<TopProductDTO> TopProducts { get; set; } = new();
        public List<TopCustomerDTO> TopCustomers { get; set; } = new();
        public int ActivePromotions { get; set; }
        public decimal TotalDiscountGiven { get; set; }
        public int PromotionUsageCount { get; set; }
    }

    public class DashboardRevenueDTO
    {
        public decimal Today { get; set; }
        public decimal Week { get; set; }
        public decimal Month { get; set; }
        public decimal Year { get; set; }
    }

    public class DashboardOrderDTO
    {
        public int New { get; set; }
        public int PendingConfirmation { get; set; }
        public int Preparing { get; set; }
        public int Arranging { get; set; }
        public int ReadyForDelivery { get; set; }
        public int Delivering { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }

    public class DashboardCustomerDTO
    {
        public int Total { get; set; }
        public int New { get; set; }
        public int Active { get; set; }
        public int Locked { get; set; }
    }

    public class DashboardProductStatsDTO
    {
        public int Total { get; set; }
        public int Active { get; set; }
        public int OutOfStock { get; set; }
        public int Discontinued { get; set; }
    }

    public class DashboardPaymentStatsDTO
    {
        public int VnPay { get; set; }
        public int Transfer { get; set; }
        public int Cash { get; set; }
        public int Pending { get; set; }
        public int Failed { get; set; }
        public int Refunded { get; set; }
    }

    public class DashboardInventoryDTO
    {
        public int InStock { get; set; }
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
    }

    public class DashboardReviewDTO
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int LatestReviews { get; set; }
    }

    public class DashboardBannerDTO
    {
        public int Active { get; set; }
        public int Expired { get; set; }
    }

    public class DashboardNotificationDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class TopProductDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopCustomerDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class DashboardRevenueChartDTO
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Data { get; set; } = new();
    }

    public class DashboardOrderChartDTO
    {
        public int Pending { get; set; }
        public int Preparing { get; set; }
        public int Delivering { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }

    public class DashboardPaymentChartDTO
    {
        public int VnPay { get; set; }
        public int Transfer { get; set; }
        public int Cash { get; set; }
    }

    public class DashboardCategoryRevenueDTO
    {
        public List<CategoryRevenueItem> Items { get; set; } = new();
    }

    public class CategoryRevenueItem
    {
        public string? CategoryName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DashboardChartsDTO
    {
        public DashboardRevenueChartDTO Revenue { get; set; } = new();
        public DashboardOrderChartDTO Orders { get; set; } = new();
        public DashboardPaymentChartDTO Payments { get; set; } = new();
        public DashboardCategoryRevenueDTO CategoryRevenue { get; set; } = new();
    }

    public class DashboardPromotionStatsDTO
    {
        public int ActiveCampaigns { get; set; }
        public int TotalCampaigns { get; set; }
        public decimal TotalDiscountGiven { get; set; }
        public int ActiveFlashSales { get; set; }
    }
}
