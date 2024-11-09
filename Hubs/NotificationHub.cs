using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class NotificationHub(NotificationService notificationService) : Hub
{
    public async Task GetNotifications()
    {
        await Clients.All.SendAsync(
            method: "ReceiveNotifications",
            arg1: await notificationService.GetUserNotifications()
        );
    }

    public async Task SendNotification(string user, string message, int type)
    {
        await Clients.All.SendAsync(method: "ReceiveNotification", arg1: user, arg2: message, arg3: type);
    }
}