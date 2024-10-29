using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GallifreyPlanet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NotificationService _notificationService;
        private readonly UserService _userService;

        public HomeController(
            ILogger<HomeController> logger,
            NotificationService notificationService,
            UserService userService
        )
        {
            _logger = logger;
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            User? user = await _userService.GetCurrentUserAsync();

            return View(user);
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
            await _notificationService.CreateNotification(user: "username", message: "Your message here");
            return Ok();
        }
    }
}
