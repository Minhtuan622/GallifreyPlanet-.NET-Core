using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task SendMessage(string chatId, string senderId, string content)
        {
            var members = await _chatService.CheckPermission(int.Parse(chatId), senderId);
            if (members is not null && await _chatService.SaveMessage(int.Parse(chatId), senderId, content))
            {
                foreach (var member in members)
                {
                    if (member?.Id != null)
                    {
                        await Clients.User(member.Id).SendAsync("ReceiveMessage", senderId, content);
                    }
                }
            }
        }

        public async Task SendMessageToCaller(string user, string message)
        {
            await Clients.Caller.SendAsync(method: "ReceiveMessage", user, message);
        }

        public async Task SendMessageToGroup(string user, string message)
        {
            await Clients
                .Group(groupName: "SignalR Users")
                .SendAsync(method: "ReceiveMessage", user, message);
        }
    }
}