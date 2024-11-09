using GallifreyPlanet.Data;
using GallifreyPlanet.Hubs;
using GallifreyPlanet.Models;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Services;

public class NotificationService(GallifreyPlanetContext context, IHubContext<NotificationHub> hubContext, UserService userService)
{
    public async Task CreateNotification(string userId, string message, NotificationType type)
    {
        var notification = new Notification
        {
            SenderId = userId,
            Type = type,
            Content = message,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        context.Notification.Add(entity: notification);
        await context.SaveChangesAsync();

        await hubContext.Clients.All.SendAsync(method: "ReceiveNotification", arg1: userId, arg2: message, arg3: type);
    }

    public async Task<List<Notification>> GetUserNotifications()
    {
        var user = await userService.GetCurrentUserAsync(); 
        
        return context.Notification
            .Where(predicate: n => n.SenderId == user!.Id)
            .OrderByDescending(keySelector: n => n.CreatedAt)
            .ToList();
    }

    public async Task MarkAsRead(int notificationId)
    {
        var notification = await context.Notification.FindAsync(keyValues: notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await context.SaveChangesAsync();
        }
    }
}