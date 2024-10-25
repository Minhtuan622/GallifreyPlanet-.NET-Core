using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string user, string message)
        {
            await Clients.All.SendAsync(method: "ReceiveNotification", user, message);
        }
    }
}
