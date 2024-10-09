using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class AccountManagerController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccountSetting()
        {
            return View();
        }
    }
}
