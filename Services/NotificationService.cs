using GallifreyPlanet.Data;
using GallifreyPlanet.Hubs;
using GallifreyPlanet.Models;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Services
{
    public class NotificationService
    {
        private readonly GallifreyPlanetContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(GallifreyPlanetContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task CreateNotification(string user, string message)
        {
            Notification? notification = new Notification
            {
                UserId = user,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync(method: "ReceiveNotification", user, message);
        }

        public List<Notification> GetUserNotifications(string user)
        {
            return _context.Notification
                .Where(n => n.UserId == user)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public async Task MarkAsRead(int notificationId)
        {
            Notification? notification = await _context.Notification.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
