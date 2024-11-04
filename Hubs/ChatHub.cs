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
            try 
            {
                if (string.IsNullOrEmpty(chatId) || !int.TryParse(chatId, out int chatIdInt))
                {
                    throw new HubException("Invalid chatId");
                }

                if (string.IsNullOrEmpty(senderId))
                {
                    throw new HubException("SenderId is required");
                }

                Message chatMessage = new Message
                {
                    ChatId = chatIdInt,
                    SenderId = senderId,
                    Content = content,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var members = await _chatService.GetMembers(chatIdInt);
                if (!members.Any(m => m != null && m.Id == senderId))
                {
                    throw new HubException("User does not have access to this chat");
                }

                await _context.ChatMessage.AddAsync(chatMessage);
                await _context.SaveChangesAsync();

                foreach (var member in members)
                {
                    if (member?.Id != null)
                    {
                        await Clients.User(member.Id).SendAsync("ReceiveMessage", senderId, content);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new HubException($"Error sending message: {ex.Message}");
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