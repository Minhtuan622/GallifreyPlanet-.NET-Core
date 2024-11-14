using GallifreyPlanet.Data;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Chat;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace GallifreyPlanet.Controllers;

public class ChatController(
    UserService userService,
    ChatService chatService,
    GallifreyPlanetContext context,
    FileService fileService
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        var conversations = new ChatManagerViewModel
        {
            User = user,
            Conversations = await chatService.GetConversationsByUserId(userId: user.Id),
        };

        return View(model: conversations);
    }

    [HttpGet(template: "Chat/{conversationId:int}")]
    public async Task<IActionResult> Chat(int conversationId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        var conversations = new ChatManagerViewModel
        {
            User = user,
            Conversations = await chatService.GetConversationsByUserId(userId: user.Id),
            SelectedConversation = await chatService.GetConversationById(conversationId: conversationId),
            Messages = await chatService.GetMessagesByConversationId(conversationId: conversationId),
        };

        return View(model: conversations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateConversation(string senderId, string receiverId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        if (chatService.Find(senderId: senderId, receiverId: receiverId) is not null)
        {
            return RedirectToAction(actionName: "Index", controllerName: "Chat");
        }

        if (chatService.CreateConversation(senderId: senderId, receiverId: receiverId))
        {
            return RedirectToAction(actionName: nameof(Index), controllerName: "Chat");
        }

        TempData[key: "StatusMessage"] = "Có lỗi xảy ra, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";

        return RedirectToAction(actionName: nameof(Index), controllerName: "Chat");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConversation(string senderId, string receiverId)
    {
        var conversation = chatService.Find(senderId: senderId, receiverId: receiverId);
        if (conversation is not null)
        {
            var messages = context.Message
                .Where(predicate: m => m.ChatId == conversation.Id)
                .ToList();
            context.RemoveRange(entities: messages);
            context.Conversation.Remove(entity: conversation);
            await context.SaveChangesAsync();
            TempData[key: "StatusMessage"] = "Xóa thành công";
            TempData[key: "StatusType"] = "success";
        }
        else
        {
            TempData[key: "StatusMessage"] = "Cuộc trò chuyện không tồn tại trên hệ thống.";
            TempData[key: "StatusType"] = "danger";
        }

        return RedirectToAction(actionName: nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        var conversations = new ChatManagerViewModel
        {
            User = user,
            Conversations = await chatService.GetConversationsByUserId(userId: user.Id),
        };

        return View(model: conversations);
    }

    /*[HttpPost]
    public async Task<IActionResult> Create(ChatManagerViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        // Upload avatar if provided
        var avatarPath = viewModel.GroupAvatar != null
            ? await fileService.UploadFileAsync(viewModel.GroupAvatar)
            : "/uploads/accounts/default-avatar.jpg";

        // Create the group chat entity
        var groupChat = new GroupChat
        {
            Name = viewModel.GroupName,
            Avatar = avatarPath,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User.Identity.Name
        };

        // Add selected members to the group chat
        foreach (var memberId in viewModel.SelectedMemberIds)
        {
            groupChat.Members.Add(new GroupChatMember
            {
                UserId = memberId,
                JoinedAt = DateTime.UtcNow
            });
        }

        // Save the group chat and its members to the database
        await context.GroupChats.AddAsync(groupChat);
        await context.SaveChangesAsync();

        return RedirectToAction("Chat", new { conversationId = groupChat.Id });
    }

    [HttpGet("api/Users/Search")]
    public async Task<IActionResult> SearchUsers(string term)
    {
        var users = await context.Users
            .Where(u => u.Name.Contains(term) || u.Email.Contains(term))
            .Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                avatar = u.Avatar ?? "/uploads/accounts/default-avatar.jpg"
            })
            .Take(10)
            .ToListAsync();

        return Json(users);
    }*/
}