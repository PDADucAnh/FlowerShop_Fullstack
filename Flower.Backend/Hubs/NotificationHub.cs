using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Flower.Backend.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                // Check if admin or staff
                if (user.IsInRole("Admin") || user.IsInRole("Staff"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
                }
                
                // Also check if they are a customer (JWT claims or NameIdentifier)
                var customerIdClaim = user.FindFirst("CustomerId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
                if (customerIdClaim != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Customer_{customerIdClaim.Value}");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Context.User;
            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                if (user.IsInRole("Admin") || user.IsInRole("Staff"))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
                }
                
                var customerIdClaim = user.FindFirst("CustomerId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
                if (customerIdClaim != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Customer_{customerIdClaim.Value}");
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
