using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class NotificationController(NotificationService notificationService) : Controller
{
    public async Task<IActionResult> SendNotification()
    {
        await notificationService.CreateNotification(user: "username", message: "Your message here");
        return Ok();
    }
}