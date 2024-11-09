using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class CommentHub(
    CommentService commentService
) : Hub
{
    public async Task SendComment(int commentableId, string content)
    {
        await Clients.All.SendAsync(
            method: "ReceiveComment",
            arg1: commentableId,
            arg2: content,
            arg3: await commentService.Add(commentableId: commentableId, content: content)
        );
    }

    public async Task ReplyComment(int parentCommentId, string content)
    {
        await Clients.All.SendAsync(
            method: "ReceiveReply", 
            arg1: parentCommentId, 
            arg2: content,
            arg3: await commentService.AddReply(commentId: parentCommentId, content: content)
        );
    }
}