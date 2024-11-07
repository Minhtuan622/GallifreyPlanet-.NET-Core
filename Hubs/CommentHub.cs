using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class CommentHub(
    CommentService commentService
) : Hub
{
    public async Task SendComment(int commentableId, string content, string userName)
    {
        var result = commentService.Add(commentableId, content);

        if (result.Result)
        {
            await Clients.All.SendAsync("ReceiveComment", commentableId, content, userName);
        }
    }

    public async Task ReplyToComment(int parentCommentId, string content, string userName)
    {
        await Clients.All.SendAsync("ReceiveReply", parentCommentId, content, userName);
    }
}