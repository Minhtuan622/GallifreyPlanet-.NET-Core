using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GallifreyPlanet.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly GallifreyPlanetContext _context;
		private readonly IEmailSender _emailSender;

		public AccountController(
			SignInManager<User> signInManager,
			UserManager<User> userManager,
			GallifreyPlanetContext context,
			IEmailSender emailSender
		)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_context = context;
			_emailSender = emailSender;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		public bool IsValidEmail(string emailaddress)
		{
			try
			{
				MailAddress m = new MailAddress(emailaddress);
				return true;
			}
			catch (FormatException)
			{
				return false;
			}
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel user)
		{
			if (ModelState.IsValid)
			{
				string? userNameOrEmail = user.UsernameOrEmail;

				if (IsValidEmail(user.UsernameOrEmail!))
				{
					User? getUser = await _userManager.FindByEmailAsync(user.UsernameOrEmail!);
					if (getUser != null)
					{
						userNameOrEmail = getUser.UserName;
					}
				}

				SignInResult? result = await _signInManager.PasswordSignInAsync(
					userNameOrEmail!,
					user.Password!,
					user.RememberMe,
					lockoutOnFailure: false
				);

				if (result.IsNotAllowed)
				{
					ModelState.AddModelError(key: "", errorMessage: "Xác thực Email để đăng nhập.");
					return View(user);
				}

				if (result.Succeeded)
				{
					return RedirectToAction(actionName: "Index", controllerName: "Home");
				}

				ModelState.AddModelError(key: "", errorMessage: "Vui lòng kiểm tra tên người dùng hoặc mật khẩu.");
			}

			return View(user);
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(SignupViewModel user)
		{
			if (ModelState.IsValid)
			{
				User? existingUser = _context.User.FirstOrDefault(u => u.Email == user.Email);

				if (existingUser != null)
				{
					ModelState.AddModelError(key: "", errorMessage: "Email đã tồn tại.");
					return View(user);
				}

				User? newUser = new User
				{
					Name = user.Name,
					UserName = user.UserName,
					Email = user.Email,
					EmailConfirmed = true,
				};

				IdentityResult? result = await _userManager.CreateAsync(newUser, user.Password!);

				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(newUser, isPersistent: false);

					return RedirectToAction(actionName: "Index", controllerName: "Home");
				}

				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError(key: "", error.Description);
				}
			}

			return View(user);
		}

		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				User? user = await _userManager.FindByEmailAsync(viewModel.Email!);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					return RedirectToAction(actionName: "ForgotPasswordConfirmation", controllerName: "Account");
				}

				string? code = await _userManager.GeneratePasswordResetTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				string? callbackUrl = Url.Page(
				pageName: "/Account/ResetPassword",
					pageHandler: null,
					values: new { area = "Identity", code },
					protocol: Request.Scheme
				);

				await _emailSender.SendEmailAsync(
					viewModel.Email!,
					subject: "Khôi phục mật khẩu",
					htmlMessage: $"Để khôi phục mật khẩu của bạn, vui lòng <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>nhấp vào đây</a>."
				);

				return RedirectToAction(actionName: "ForgotPasswordConfirmation", controllerName: "Account");
			}

			return View(viewModel);
		}

		[HttpGet]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		public IActionResult ResetPassword()
		{
			return View();
		}

		[HttpGet]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		[HttpPost]
		public IActionResult ResetPassword(ResetPasswordViewModel viewModel)
		{
			return View(viewModel);
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(actionName: "Index", controllerName: "Home");
		}
	}
}
