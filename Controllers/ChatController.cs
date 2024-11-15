using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Chat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
            Conversations = await chatService.GetConversationsByUserId(user.Id),
        };

        return View(conversations);
    }

    [HttpGet("Chat/{conversationId:int}")]
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
            Conversations = await chatService.GetConversationsByUserId(user.Id),
            SelectedConversation = await chatService.GetConversationById(conversationId),
            Messages = await chatService.GetMessagesByConversationId(conversationId),
        };

        return View(conversations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateConversation(string senderId, string receiverId)
    {
        var user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        if (chatService.Find(senderId, receiverId) is not null)
        {
            return RedirectToAction("Index");
        }

        if (chatService.CreateConversation(senderId, receiverId))
        {
            return RedirectToAction(nameof(Index));
        }

        TempData["StatusMessage"] = "Có lỗi xảy ra, vui lòng thử lại sau";
        TempData["StatusType"] = "danger";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConversation(string senderId, string receiverId)
    {
        var conversation = chatService.Find(senderId, receiverId);
        if (conversation is not null)
        {
            var messages = context.Message
                .Where(m => m.ChatId == conversation.Id)
                .ToList();

            context.RemoveRange(messages);
            context.Conversation.Remove(conversation);
            await context.SaveChangesAsync();

            TempData["StatusMessage"] = "Xóa thành công";
            TempData["StatusType"] = "success";
        }
        else
        {
            TempData["StatusMessage"] = "Cuộc trò chuyện không tồn tại trên hệ thống.";
            TempData["StatusType"] = "danger";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> CreateGroup()
    {
        var user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        var conversations = new ChatManagerViewModel
        {
            User = user,
            Conversations = await chatService.GetConversationsByUserId(user.Id),
        };

        return View(conversations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup(ChatManagerViewModel viewModel)
    {
        if (!ModelState.IsValid || viewModel.NewConversation is null)
        {
            return View(viewModel);
        }

        var groupChat = new Conversation
        {
            IsGroup = true,
            GroupName = viewModel.NewConversation.GroupName,
            GroupDetail = viewModel.NewConversation.GroupDetail,
            GroupAvatar = viewModel.NewConversation.GroupAvatar is not null
                ? await fileService.UploadFileAsync(viewModel.NewConversation.GroupAvatar, "conversations")
                : null,
            CreatedBy = viewModel.User?.Id,
            CreatedAt = DateTime.Now
        };

        // Serialize the selected member IDs as JSON
        if (viewModel.SelectedMemberIds is not null && viewModel.SelectedMemberIds.Any())
        {
            groupChat.Members = JsonSerializer.Serialize(viewModel.SelectedMemberIds);
        }

        await context.Conversation.AddAsync(groupChat);
        await context.SaveChangesAsync();

        TempData["StatusMessage"] = "Nhóm đã được tạo thành công!";
        TempData["StatusType"] = "success";

        return RedirectToAction("Chat", new { conversationId = groupChat.Id });
    }

    [HttpGet("api/Users/Search/{search}")]
    public async Task<IActionResult> SearchUsers(string search)
    {
        var users = await context.Users
            .Where(u =>
                u.UserName != null &&
                u.Name != null &&
                u.Email != null && (
                    u.UserName.Contains(search) ||
                    u.Name.Contains(search) ||
                    u.Email.Contains(search)
                )
            )
            .Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                avatar = u.Avatar
            })
            .ToListAsync();

        return Json(users);
    }
}