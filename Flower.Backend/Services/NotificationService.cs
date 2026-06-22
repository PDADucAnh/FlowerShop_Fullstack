using Flower.Backend.Hubs;
using Flower.Backend.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Flower.Backend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyEntityChanged(string entityName)
        {
            await _hubContext.Clients.All.SendAsync("EntityChanged", entityName);
        }
    }
}
