using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class CommentHub : Hub
{
    public async Task SendComment(string userId, string message)
    {
        await Clients.All.SendAsync(method: "ReceiveComment", arg1: userId, arg2: message);
    }

    public async Task ReplyToComment(string userId, int parentCommentId, string message)
    {
        await Clients.All.SendAsync(method: "ReceiveChildComment", arg1: userId, arg2: parentCommentId, arg3: message);
    }
}