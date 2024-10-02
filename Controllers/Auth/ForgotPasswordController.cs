using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers.Auth
{
	public class ForgotPasswordController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
