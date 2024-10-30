using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Security.Claims;
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
        private readonly UserService _userService;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            GallifreyPlanetContext context,
            IEmailSender emailSender,
            UserService userService
        )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
            _userService = userService;
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
                    User? currentUser = await _userService.GetCurrentUserAsync();
                    await _userService.AddLoginHistoryAsync(
                        currentUser!.Id,
                        HttpContext.Connection.RemoteIpAddress?.ToString()!,
                        HttpContext.Request.Headers[key: "User-Agent"].ToString()
                    );

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
                User? existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);

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
                    ShowEmail = false,
                    AllowMessagesFromNonFriends = false,
                    EmailNotifications = false,
                    PushNotifications = false,
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
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAuthenticatorCode(bool rememberMe, string? returnUrl = null)
        {
            User? user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View(viewName: "Error");
            }
            ViewData[index: "ReturnUrl"] = returnUrl;

            return View(model: new VerifyAuthenticatorViewModel { ReturnUrl = returnUrl, RememberMe = rememberMe });

        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnurl = null)
        {
            string? redirectUrl = Url.Action(action: "ExternalLoginCallback", controller: "Account", new { returnurl });
            AuthenticationProperties? properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnurl = null, string? remoteError = null)
        {
            returnurl = returnurl ?? Url.Content(contentPath: "~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage: $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            //sign in the user with this external login provider. only if they have a login
            SignInResult? result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                               isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnurl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(VerifyAuthenticatorCode), new { returnurl });
            }
            else
            {
                //that means user account is not create and we will display a view to create an account

                ViewData[index: "ReturnUrl"] = returnurl;
                ViewData[index: "ProviderDisplayName"] = info.ProviderDisplayName;

                return View(viewName: "ExternalLoginConfirmation", model: new ExternalLoginConfirmationViewModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Name = info.Principal.FindFirstValue(ClaimTypes.Name)
                });
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginConfirmation(
            ExternalLoginConfirmationViewModel model,
            string? returnurl = null
        )
        {
            returnurl ??= Url.Content(contentPath: "~/");

            if (ModelState.IsValid)
            {
                ExternalLoginInfo? info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View(viewName: "Error");
                }

                User? user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Email,
                    EmailConfirmed = true,
                    ShowEmail = false,
                    AllowMessagesFromNonFriends = false,
                    EmailNotifications = false,
                    PushNotifications = false,
                };

                IdentityResult? result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnurl);
                    }
                }
                AddErrors(result);
            }
            ViewData[index: "ReturnUrl"] = returnurl;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
