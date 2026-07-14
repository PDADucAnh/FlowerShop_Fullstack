using Flower.Backend.Hubs;
using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task NotifyEntityChanged(string entityName)
        {
            await _hubContext.Clients.All.SendAsync("EntityChanged", entityName);
        }

        public async Task NotifyCustomerEvent(int customerId, string eventName, object data = null)
        {
            await _hubContext.Clients.Group($"Customer_{customerId}").SendAsync(eventName, data);
        }

        public async Task NotifyBroadcastEvent(string eventName, object data = null)
        {
            await _hubContext.Clients.All.SendAsync(eventName, data);
        }

        public async Task CreateCustomerNotification(int customerId, string title, string content, string type, 
            int? orderId = null, string? referenceType = null, string? icon = null, 
            string? priority = "Normal", string? navigationUrl = null, string? metadata = null)
        {
            var notification = new Notification
            {
                CustomerId = customerId,
                Title = title,
                Content = content,
                Type = type,
                OrderId = orderId,
                ReferenceType = referenceType,
                Icon = icon,
                Priority = priority,
                NavigationUrl = navigationUrl,
                Metadata = metadata,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Push to SignalR
            await _hubContext.Clients.Group($"Customer_{customerId}").SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                content = notification.Content,
                type = notification.Type,
                isRead = notification.IsRead,
                createdAt = notification.CreatedAt,
                navigationUrl = notification.NavigationUrl,
                icon = notification.Icon
            });
        }

        public async Task<(List<Notification> Items, int TotalCount)> GetCustomerNotifications(int customerId, int page, int pageSize)
        {
            var query = _context.Notifications.Where(n => n.CustomerId == customerId);
            
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<int> GetCustomerUnreadCount(int customerId)
        {
            return await _context.Notifications
                .CountAsync(n => n.CustomerId == customerId && !n.IsRead);
        }

        public async Task<bool> MarkAsRead(int id, int customerId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.CustomerId == customerId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                // Realtime update badge
                var unreadCount = await GetCustomerUnreadCount(customerId);
                await _hubContext.Clients.Group($"Customer_{customerId}").SendAsync("UnreadCountChanged", unreadCount);
                return true;
            }
            return false;
        }

        public async Task MarkAllAsRead(int customerId)
        {
            var unread = await _context.Notifications.Where(n => n.CustomerId == customerId && !n.IsRead).ToListAsync();
            var now = DateTime.UtcNow;
            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadAt = now;
            }
            if (unread.Any())
            {
                await _context.SaveChangesAsync();
                await _hubContext.Clients.Group($"Customer_{customerId}").SendAsync("UnreadCountChanged", 0);
            }
        }
    }
}
