using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(string user, string message, int type)
    {
        await Clients.All.SendAsync(method: "ReceiveNotification", arg1: user, arg2: message, arg3: type);
    }
}