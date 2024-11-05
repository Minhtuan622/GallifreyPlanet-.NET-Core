using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Chat;

namespace GallifreyPlanet.Services
{
    public class ChatService
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;

        public ChatService(
            GallifreyPlanetContext context,
            UserService userService
        )
        {
            _context = context;
            _userService = userService;
        }

        public bool CreateConversation(string senderId, string receiverId, string? groupName = null)
        {
            var existingConversation = _context.Conversation
                .FirstOrDefault(c =>
                    c.Members != null &&
                    c.Members.Contains(senderId) &&
                    c.Members.Contains(receiverId) &&
                    c.IsGroup == false
                );

            if (existingConversation is not null)
            {
                return false;
            }

            var conversation = new Conversation()
            {
                Members = string.Join(",", new List<string> { senderId, receiverId }),
                GroupName = groupName,
                IsGroup = false,
                CreatedAt = DateTime.Now,
            };

            _context.Conversation.Add(conversation);
            _context.SaveChanges();

            return true;
        }

        public async Task<bool> SaveMessage(int chatId, string senderId, string content)
        {
            if (string.IsNullOrEmpty(senderId))
            {
                return false;
            }

            Message chatMessage = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.ChatMessage.AddAsync(chatMessage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User?>?> CheckPermission(int chatId, string senderId)
        {
            var members = await GetMembers(chatId);
            return members.Any(m => m != null && m.Id == senderId) ? members : null;
        }

        public Task<ConversationViewModel> GetConversationById(int conversationId)
        {
            return NewConversationViewModel(_context.Conversation.FirstOrDefault(c => c.Id == conversationId)!);
        }

        public async Task<List<ConversationViewModel>> GetConversationsByUserId(string userId)
        {
            var conversations = _context.Conversation
                .Where(c =>
                    c.Members != null &&
                    c.Members.Contains(userId)
                )
                .OrderBy(c => c.CreatedAt)
                .ToList();
            var newConversations = new List<ConversationViewModel>();

            foreach (Conversation conversation in conversations)
            {
                newConversations.Add(await NewConversationViewModel(conversation));
            }

            return newConversations;
        }

        public async Task<List<MessageViewModel>> GetMessagesByConversationId(int conversationId)
        {
            var messages = _context.Message
                .Where(m => m.ChatId == conversationId)
                .ToList();
            var newMessagesViewModels = new List<MessageViewModel>();

            foreach (Message message in messages)
            {
                newMessagesViewModels.Add(await NewMessageViewModel(message));
            }

            return newMessagesViewModels;
        }

        public string? GetLatestMessage(int conversationId)
        {
            return _context.Message
                .OrderBy(m => m.CreatedAt)
                .LastOrDefault(m => m.ChatId == conversationId)!
                .Content;
        }

        private async Task<List<User?>> GetMembers(int chatId)
        {
            var conversation = _context.Conversation.FirstOrDefault(c => c.Id == chatId);
            var members = conversation?.Members;
            var usersId = members!.Split(',');
            var usersList = new List<User?>();

            foreach (string userId in usersId)
            {
                usersList.Add(await _userService.GetUserAsyncById(userId));
            }

            return usersList;
        }

        private async Task<ConversationViewModel> NewConversationViewModel(Conversation conversation)
        {
            return new ConversationViewModel
            {
                Id = conversation.Id,
                Members = await GetMembers(conversation.Id),
                GroupName = conversation.GroupName,
                IsGroup = conversation.IsGroup,
                CreatedAt = conversation.CreatedAt,
                LatestMessage = GetLatestMessage(conversation.Id)
            };
        }

        private async Task<MessageViewModel> NewMessageViewModel(Message message)
        {
            return new MessageViewModel
            {
                Conversation = await GetConversationById(message.ChatId),
                Sender = await _userService.GetUserAsyncById(message.SenderId!),
                Content = message.Content,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt,
                UpdatedAt = message.UpdatedAt,
            };
        }
    }
}