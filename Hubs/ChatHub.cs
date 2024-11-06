using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs;

public class ChatHub(ChatService chatService) : Hub
{
    public async Task SendMessage(string chatId, string senderId, string content)
    {
        var members = await chatService.CheckPermission(chatId: int.Parse(s: chatId), senderId: senderId);
        if (members is not null && await chatService.SaveMessage(chatId: int.Parse(s: chatId), senderId: senderId, content: content))
        {
            foreach (var member in members)
            {
                if (member?.Id != null)
                {
                    await Clients.User(userId: member.Id).SendAsync(method: "ReceiveMessage", arg1: senderId, arg2: content);
                }
            }
        }
    }

    public async Task SendMessageToCaller(string user, string message)
    {
        await Clients.Caller.SendAsync(method: "ReceiveMessage", arg1: user, arg2: message);
    }

    public async Task SendMessageToGroup(string user, string message)
    {
        await Clients
            .Group(groupName: "SignalR Users")
            .SendAsync(method: "ReceiveMessage", arg1: user, arg2: message);
    }
}