using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Chat;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class ChatController(
    UserService userService,
    ChatService chatService
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

        // todo: improve create logic 
        var result = chatService.CreateConversation(senderId: senderId, receiverId: receiverId);
        if (!result)
        {
            return NotFound(value: "Could not create conversation, the conversation already exists.");
        }

        return RedirectToAction(actionName: nameof(Index), controllerName: "Chat");
    }
}