using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers.Auth
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
