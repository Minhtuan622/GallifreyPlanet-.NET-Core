using GallifreyPlanet.Data;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Chat;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class ChatController(
    UserService userService,
    ChatService chatService,
    GallifreyPlanetContext context
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

        if (!chatService.CreateConversation(senderId: senderId, receiverId: receiverId))
        {
            ModelState.AddModelError(key: string.Empty, errorMessage: "Có lỗi xảy ra, vui lòng thử lại sau");
        }

        return RedirectToAction(actionName: nameof(Index), controllerName: "Chat");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConversation(string senderId, string receiverId)
    {
        var conversation = chatService.Find(senderId: senderId, receiverId: receiverId);
        if (conversation is not null)
        {
            context.Conversation.Remove(conversation);
            await context.SaveChangesAsync();
            TempData[key: "StatusMessage"] = "Xóa thành công";
            TempData[key: "StatusType"] = "success";
        }
        else
        {
            ModelState.AddModelError(key: string.Empty, errorMessage: "Cuộc trò chuyện không tồn tại trên hệ thống.");
        }

        return RedirectToAction(actionName: nameof(Index));
    }
}