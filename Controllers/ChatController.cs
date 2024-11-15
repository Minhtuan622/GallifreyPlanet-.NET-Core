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
            return RedirectToAction(actionName: "Index");
        }

        if (chatService.CreateConversation(senderId: senderId, receiverId: receiverId))
        {
            return RedirectToAction(actionName: nameof(Index));
        }

        TempData[key: "StatusMessage"] = "Có lỗi xảy ra, vui lòng thử lại sau";
        TempData[key: "StatusType"] = "danger";

        return RedirectToAction(actionName: nameof(Index));
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
            Conversations = await chatService.GetConversationsByUserId(userId: user.Id),
        };

        return View(model: conversations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup(ChatManagerViewModel viewModel)
    {
        if (!ModelState.IsValid || viewModel.NewConversation is null)
        {
            return View(model: viewModel);
        }

        var groupChat = new Conversation
        {
            IsGroup = true,
            GroupName = viewModel.NewConversation.GroupName,
            GroupDetail = viewModel.NewConversation.GroupDetail,
            GroupAvatar = viewModel.NewConversation.GroupAvatar is not null
                ? await fileService.UploadFileAsync(file: viewModel.NewConversation.GroupAvatar, folder: "conversations")
                : null,
            CreatedBy = viewModel.User?.Id,
            CreatedAt = DateTime.Now
        };

        // Serialize the selected member IDs as JSON
        if (viewModel.SelectedMemberIds is not null && viewModel.SelectedMemberIds.Any())
        {
            groupChat.Members = JsonSerializer.Serialize(value: viewModel.SelectedMemberIds);
        }

        await context.Conversation.AddAsync(entity: groupChat);
        await context.SaveChangesAsync();

        TempData[key: "StatusMessage"] = "Nhóm đã được tạo thành công!";
        TempData[key: "StatusType"] = "success";

        return RedirectToAction(actionName: "Chat", routeValues: new { conversationId = groupChat.Id });
    }

    [HttpGet(template: "api/Users/Search/{search}")]
    public async Task<IActionResult> SearchUsers(string search)
    {
        var users = await context.Users
            .Where(predicate: u =>
                u.UserName != null &&
                u.Name != null &&
                u.Email != null && (
                    u.UserName.Contains(search) ||
                    u.Name.Contains(search) ||
                    u.Email.Contains(search)
                )
            )
            .Select(selector: u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                avatar = u.Avatar
            })
            .ToListAsync();

        return Json(data: users);
    }
}