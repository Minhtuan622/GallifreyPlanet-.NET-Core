using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class NotificationHub(NotificationService notificationService) : Hub
{
    public async Task GetNotifications()
    {
        await Clients.All.SendAsync(
            method: "ReceiveNotifications",
            arg1: await notificationService.GetByUser()
        );
    }

    public async Task SendNotification(string user, string message, int type)
    {
        await Clients.All.SendAsync(method: "ReceiveNotification", arg1: user, arg2: message, arg3: type);
    }

    public async Task MarkAllAsRead(List<int> ids)
    {
        if (ids.Count > 0)
        {
            foreach (var id in ids)
            {
                await notificationService.MarkAsRead(id);
            }

            await Clients.All.SendAsync("MarkAllAsRead", ids);
        }
    }

    public async Task DeleteAll(List<int> ids)
    {
        if (ids.Count > 0)
        {
            foreach (var id in ids)
            {
                await notificationService.Delete(id);
            }
            
            await Clients.All.SendAsync("DeleteAll", ids);
        }
    }
}