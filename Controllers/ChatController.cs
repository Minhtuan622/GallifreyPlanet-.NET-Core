using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers;

public class ChatController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}