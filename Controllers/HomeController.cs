using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GallifreyPlanet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationService _notificationService;

        public HomeController(ILogger<HomeController> logger, INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> SendNotification()
        {
            await _notificationService.CreateNotification("username", "Your message here");
            return Ok();
        }
    }
}
