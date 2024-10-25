using GallifreyPlanet.Data;
using GallifreyPlanet.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Services
{
    public class Notification
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? User { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public interface INotificationService
    {
        Task CreateNotification(string user, string message);
        Task<List<Notification>> GetUserNotifications(string user);
        Task MarkAsRead(int notificationId);
    }

    public class NotificationService : INotificationService
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
            var notification = new Notification
            {
                User = user,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", user, message);
        }

        public async Task<List<Notification>> GetUserNotifications(string user)
        {
            return await _context.Notifications
                .Where(n => n.User == user)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsRead(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
