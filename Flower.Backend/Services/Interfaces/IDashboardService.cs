using Flower.Backend.Models.DTOs;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDTO> GetSummary();
        Task<DashboardRevenueDTO> GetRevenue();
        Task<DashboardOrderDTO> GetOrders();
        Task<DashboardProductStatsDTO> GetProducts();
        Task<DashboardCustomerDTO> GetCustomers();
        Task<DashboardChartsDTO> GetCharts();
        Task<List<DashboardNotificationDTO>> GetNotifications();
    }
}
