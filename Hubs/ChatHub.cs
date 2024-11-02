using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly GallifreyPlanetContext _context;

        public ChatHub(GallifreyPlanetContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string chatId, string senderId, string receiverId, string content)
        {
            Message chatMessage = new Message
            {
                ChatId = int.Parse(chatId),
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ChatMessage.Add(chatMessage);
            await _context.SaveChangesAsync();

            await Clients.User(receiverId).SendAsync(method: "ReceiveMessage", senderId, content);
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