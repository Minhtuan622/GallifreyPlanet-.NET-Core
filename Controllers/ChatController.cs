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
        User? user = await userService.GetCurrentUserAsync();
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
        User? user = await userService.GetCurrentUserAsync();
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
        User? user = await userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        // todo: improve create logic 
        bool result = chatService.CreateConversation(senderId, receiverId);
        if (!result)
        {
            return NotFound("Could not create conversation, the conversation already exists.");
        }

        return RedirectToAction(nameof(Index), controllerName: "Chat");
    }
}