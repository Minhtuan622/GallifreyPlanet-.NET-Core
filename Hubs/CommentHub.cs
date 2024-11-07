using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class CommentHub(
    CommentService commentService
) : Hub
{
    public async Task SendComment(int commentableId, string content, string userName)
    {
        try
        {
            await Clients.All.SendAsync(
                method: "ReceiveComment",
                arg1: commentableId,
                arg2: content,
                arg3: commentService.Add(commentableId: commentableId, content: content)
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task ReplyToComment(int parentCommentId, string content, string userName)
    {
        await Clients.All.SendAsync(method: "ReceiveReply", arg1: parentCommentId, arg2: content, arg3: userName);
    }
}