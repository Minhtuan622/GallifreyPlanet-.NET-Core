using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.SignalR;

namespace GallifreyPlanet.Hubs
{
    public class ChatHub : Hub
    {
        private readonly GallifreyPlanetContext _context;
        private readonly ChatService _chatService;

        public ChatHub(GallifreyPlanetContext context, ChatService chatService)
        {
            _context = context;
            _chatService = chatService;
        }

        public async Task SendMessage(string chatId, string senderId, string content)
        {
            Message chatMessage = new Message
            {
                ChatId = int.Parse(chatId),
                SenderId = senderId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ChatMessage.AddAsync(chatMessage);
            await _context.SaveChangesAsync();

            var members = await _chatService.GetMembers(int.Parse(chatId));

            foreach (var member in members)
            {
                await Clients.User(member!.Id).SendAsync(method: "ReceiveMessage", senderId, content);
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