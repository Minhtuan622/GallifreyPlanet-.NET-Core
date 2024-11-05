using GallifreyPlanet.Data;
using GallifreyPlanet.Hubs;
using GallifreyPlanet.Models;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Services
{
    public class NotificationService(GallifreyPlanetContext context, IHubContext<NotificationHub> hubContext)
    {
        public async Task CreateNotification(string user, string message)
        {
            Notification notification = new Notification
            {
                UserId = user,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            context.Notification.Add(notification);
            await context.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync(method: "ReceiveNotification", user, message);
        }

        public List<Notification> GetUserNotifications(string user)
        {
            return context.Notification
                .Where(n => n.UserId == user)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public async Task MarkAsRead(int notificationId)
        {
            Notification? notification = await context.Notification.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await context.SaveChangesAsync();
            }
        }
    }
}
