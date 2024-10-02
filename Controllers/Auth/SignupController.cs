using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers.Auth
{
    public class SignupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
