using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GallifreyPlanet.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<User> _signInManager;
		private readonly GallifreyPlanetContext _context;

		public AccountController(SignInManager<User> signInManager, GallifreyPlanetContext context)
		{
			_signInManager = signInManager;
			_context = context;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel user)
		{
			if (ModelState.IsValid)
			{
				SignInResult? result = await _signInManager.PasswordSignInAsync(user.Email!, user.Password!, false, false);

				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Home");
				}


			}

			ModelState.AddModelError("", "Đăng nhập không thành công.");
			return View();
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Register(SignupViewModel user)
		{
			if (ModelState.IsValid)
			{
				User? existingUser = _context.User.FirstOrDefault(u => u.Email == user.Email);

				if (existingUser != null)
				{
					ModelState.AddModelError("", "Số điện thoại đã tồn tại.");
					return View();
				}

				User? newUser = new User
				{
					//Name = user.Name,
					//Email = user.Email,
					//Password = Crypto.HashPassword(user.Password),
					//Password = user.Password,
				};

				_context.User.Add(newUser);
				_context.SaveChanges();

				return RedirectToAction("Login", "Account");
			}

			return View();
		}

		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpGet]
		public IActionResult ResetPassword()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync("cookieScheme");
			HttpContext.Session.Clear();
			HttpContext.Session.Remove("UserName");
			return RedirectToAction("Index", "Home");
		}
	}
}
