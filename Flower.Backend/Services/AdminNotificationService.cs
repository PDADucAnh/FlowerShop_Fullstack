using Flower.Backend.Services.Interfaces;
using Flower.Data;
using Flower.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class AdminNotificationService : IAdminNotificationService
    {
        private readonly ApplicationDbContext _context;

        public AdminNotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotification(string title, string message, string type, string? referenceId = null, int? userId = null)
        {
            var notification = new AdminNotification
            {
                Title = title,
                Message = message,
                Type = type,
                ReferenceId = referenceId,
                UserId = userId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.AdminNotifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AdminNotification>> GetLatestNotifications(int limit = 10)
        {
            return await _context.AdminNotifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCount()
        {
            return await _context.AdminNotifications
                .CountAsync(n => !n.IsRead);
        }

        public async Task MarkAsRead(int id)
        {
            var notification = await _context.AdminNotifications.FindAsync(id);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsRead()
        {
            var unread = await _context.AdminNotifications.Where(n => !n.IsRead).ToListAsync();
            foreach (var n in unread)
            {
                n.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<(List<AdminNotification> Items, int TotalCount)> GetAllNotifications(string? type, string? search, int page, int pageSize)
        {
            var query = _context.AdminNotifications.AsQueryable();

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(n => n.Type == type);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(n => n.Title.Contains(search) || n.Message.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
