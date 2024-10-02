using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers.Auth
{
	public class ResetPasswordController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
