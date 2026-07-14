using Flower.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flower.Backend.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyEntityChanged(string entityName);
        Task NotifyCustomerEvent(int customerId, string eventName, object data = null);
        Task NotifyBroadcastEvent(string eventName, object data = null);
        
        Task CreateCustomerNotification(int customerId, string title, string content, string type, 
            int? orderId = null, string? referenceType = null, string? icon = null, 
            string? priority = "Normal", string? navigationUrl = null, string? metadata = null);
            
        Task<(List<Notification> Items, int TotalCount)> GetCustomerNotifications(int customerId, int page, int pageSize);
        Task<int> GetCustomerUnreadCount(int customerId);
        Task<bool> MarkAsRead(int id, int customerId);
        Task MarkAllAsRead(int customerId);
    }
}
