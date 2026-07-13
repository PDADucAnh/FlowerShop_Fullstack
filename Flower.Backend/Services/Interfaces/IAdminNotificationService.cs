using Flower.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface IAdminNotificationService
    {
        Task CreateNotification(string title, string message, string type, string? referenceId = null, int? userId = null);
        Task<List<AdminNotification>> GetLatestNotifications(int limit = 10);
        Task<int> GetUnreadCount();
        Task MarkAsRead(int id);
        Task MarkAllAsRead();
        Task<(List<AdminNotification> Items, int TotalCount)> GetAllNotifications(string? type, string? search, int page, int pageSize);
    }
}
