using System.Diagnostics;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class HomeController(
        NotificationService notificationService,
        UserService userService)
        : Controller
    {
        public async Task<IActionResult> Index()
        {
            var user = await userService.GetCurrentUserAsync();

            return View(model: user);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> SendNotification()
        {
            await notificationService.CreateNotification(user: "username", message: "Your message here");
            return Ok();
        }
    }
}
