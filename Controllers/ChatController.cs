using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Chat;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class ChatController : Controller
{
    private readonly UserService _userService;
    private readonly ChatService _chatService;

    public ChatController(
        UserService userService,
        ChatService chatService
    )
    {
        _userService = userService;
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        User? user = await _userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        var conversations = new ChatManagerViewModel
        {
            Conversations = await _chatService.GetConversationsByUserId(user.Id),
        };
        
        return View(conversations);
    }

    [HttpGet("Chat/{conversationId:int}")]
    public async Task<IActionResult> Chat(int conversationId)
    {
        User? user = await _userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }
        
        var conversations = new ChatManagerViewModel
        {
            User = user,
            Conversations = await _chatService.GetConversationsByUserId(user.Id),
            SelectedConversation = await _chatService.GetConversationById(conversationId),
            Messages = await _chatService.GetMessagesByConversationId(conversationId),
        };
        
        return View(conversations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateConversation(string senderId, string receiverId)
    {
        User? user = await _userService.GetCurrentUserAsync();
        if (user is null)
        {
            return NotFound();
        }

        // todo: improve create logic 
        bool result = _chatService.CreateConversation(senderId, receiverId);
        if (!result)
        {
            return NotFound("Could not create conversation, the conversation already exists.");
        }

        return RedirectToAction(nameof(Index), controllerName: "Chat");
    }
}