using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
	public class AccountManagerController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
