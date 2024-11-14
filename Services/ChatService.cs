using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Chat;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services;

public class ChatService(
    GallifreyPlanetContext context,
    UserService userService
)
{
    public Conversation? Find(string senderId, string receiverId)
    {
        return context.Conversation
            .FirstOrDefault(predicate: c =>
                c.Members != null &&
                c.Members.Contains(senderId) &&
                c.Members.Contains(receiverId) &&
                c.IsGroup == false
            );
    }
    public bool CreateConversation(string senderId, string receiverId, string? groupName = null)
    {
        if (Find(senderId: senderId, receiverId: receiverId) is not null)
        {
            return false;
        }

        var conversation = new Conversation
        {
            Members = string.Join(separator: ",", values: new List<string> { senderId, receiverId }),
            GroupName = groupName,
            IsGroup = false,
            CreatedAt = DateTime.Now,
        };

        context.Conversation.Add(entity: conversation);
        context.SaveChanges();

        return true;
    }

    public async Task<Message?> SaveMessage(int chatId, string senderId, string content)
    {
        if (string.IsNullOrEmpty(value: senderId))
        {
            return null;
        }

        var chatMessage = new Message
        {
            ChatId = chatId,
            SenderId = senderId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.ChatMessage.AddAsync(entity: chatMessage);
        await context.SaveChangesAsync();
        return chatMessage;
    }

    public async Task<List<User?>?> CheckPermission(int chatId, string senderId)
    {
        var members = await GetMembers(chatId: chatId);
        return members.Any(predicate: m => m != null && m.Id == senderId) ? members : null;
    }

    private void MarkAsRead(Conversation conversation)
    {
        conversation.IsRead = true;

        context.Update(entity: conversation);
        context.SaveChanges();
    }

    public Task<ConversationViewModel> GetConversationById(int conversationId)
    {
        var conversation = context.Conversation.FirstOrDefault(predicate: c => c.Id == conversationId)!;
        MarkAsRead(conversation: conversation);

        return NewConversationViewModel(conversation: conversation);
    }

    public async Task<List<ConversationViewModel>> GetConversationsByUserId(string userId)
    {
        var conversations = context.Conversation
            .Where(predicate: c =>
                c.Members != null &&
                c.Members.Contains(userId)
            )
            .OrderBy(keySelector: c => c.CreatedAt)
            .ToList();
        var newConversations = new List<ConversationViewModel>();

        foreach (var conversation in conversations)
        {
            newConversations.Add(item: await NewConversationViewModel(conversation: conversation));
        }

        return newConversations;
    }

    public async Task<List<MessageViewModel>> GetMessagesByConversationId(int conversationId)
    {
        var messages = context.Message
            .Where(predicate: m => m.ChatId == conversationId)
            .ToList();
        var newMessagesViewModels = new List<MessageViewModel>();

        foreach (var message in messages)
        {
            newMessagesViewModels.Add(item: await NewMessageViewModel(message: message));
        }

        return newMessagesViewModels;
    }

    private Message? GetLatestMessage(int conversationId)
    {
        return context.Message
            .OrderBy(keySelector: m => m.CreatedAt)
            .LastOrDefault(predicate: m => m.ChatId == conversationId);
    }

    private async Task<List<User?>> GetMembers(int chatId)
    {
        var conversation = context.Conversation.FirstOrDefault(predicate: c => c.Id == chatId);
        var members = conversation?.Members;
        var usersId = members!.Split(separator: ',');
        var usersList = new List<User?>();

        foreach (var userId in usersId)
        {
            usersList.Add(item: await userService.GetUserAsyncById(userId: userId));
        }

        return usersList;
    }

    public async Task<bool> RevokeMessage(int messageId, string userId)
    {
        var message = await context.Message.FirstOrDefaultAsync(predicate: m => m.Id == messageId);
        if (message is null || message.SenderId != userId || (DateTime.UtcNow - message.CreatedAt).TotalHours > 24)
        {
            return false;
        }

        message.IsRevoked = true;
        message.RevokedAt = DateTime.UtcNow;
        message.Content = "[Tin nhắn đã bị thu hồi]";

        context.Message.Update(entity: message);
        await context.SaveChangesAsync();

        return true;
    }

    private async Task<ConversationViewModel> NewConversationViewModel(Conversation conversation)
    {
        var message = GetLatestMessage(conversationId: conversation.Id);

        return new ConversationViewModel
        {
            Id = conversation.Id,
            Members = await GetMembers(chatId: conversation.Id),
            GroupName = conversation.GroupName,
            IsGroup = conversation.IsGroup,
            IsRead = conversation.IsRead,
            CreatedAt = conversation.CreatedAt,
            LatestMessage = message?.Content,
        };
    }

    private async Task<MessageViewModel> NewMessageViewModel(Message message)
    {
        return new MessageViewModel
        {
            Id = message.Id,
            Conversation = await GetConversationById(conversationId: message.ChatId),
            Sender = await userService.GetUserAsyncById(userId: message.SenderId!),
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            IsRevoked = message.IsRevoked,
            RevokedAt = message.RevokedAt
        };
    }
}