using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class CommentHub : Hub
{
    public async Task SendComment(int commentableId, string content, string userName)
    {
        await Clients.All.SendAsync("ReceiveComment", commentableId, content, userName);
    }

    public async Task ReplyToComment(int parentCommentId, string content, string userName)
    {
        await Clients.All.SendAsync("ReceiveReply", parentCommentId, content, userName);
    }
}