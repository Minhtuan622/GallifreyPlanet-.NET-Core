using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GallifreyPlanet.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly GallifreyPlanetContext _context;

        public AccountController(
                SignInManager<User> signInManager,
                UserManager<User> userManager,
                GallifreyPlanetContext context
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
                SignInResult? result = await _signInManager.PasswordSignInAsync(user.Username!, user.Password!, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }

                ModelState.AddModelError(key: "", errorMessage: "Đăng nhập không thành công.");
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
                    UserName = user.Email,
                    Email = user.Email,
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

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
    }
}
