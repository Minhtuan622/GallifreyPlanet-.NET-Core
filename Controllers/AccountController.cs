using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
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
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GallifreyPlanet.Controllers
{
    public class AccountController(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        GallifreyPlanetContext context,
        IEmailSender emailSender,
        UserService userService)
        : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(address: emailaddress);
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

                if (IsValidEmail(emailaddress: user.UsernameOrEmail!))
                {
                    User? getUser = await userManager.FindByEmailAsync(email: user.UsernameOrEmail!);
                    if (getUser != null)
                    {
                        userNameOrEmail = getUser.UserName;
                    }
                }

                SignInResult result = await signInManager.PasswordSignInAsync(
                    userName: userNameOrEmail!,
                    password: user.Password!,
                    isPersistent: user.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(key: "", errorMessage: "Xác thực Email để đăng nhập.");
                    return View(model: user);
                }

                if (result.Succeeded)
                {
                    User? currentUser = await userService.GetCurrentUserAsync();
                    await userService.AddLoginHistoryAsync(
                        userId: currentUser!.Id,
                        ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()!,
                        userAgent: HttpContext.Request.Headers[key: "User-Agent"].ToString()
                    );

                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }

                ModelState.AddModelError(key: "", errorMessage: "Vui lòng kiểm tra tên người dùng hoặc mật khẩu.");
            }

            return View(model: user);
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
                User? existingUser = context.Users.FirstOrDefault(predicate: u => u.Email == user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError(key: "", errorMessage: "Email đã tồn tại.");
                    return View(model: user);
                }

                User newUser = new User
                {
                    Name = user.Name,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = true,
                    ShowEmail = false,
                    AllowChat = false,
                    EmailNotifications = false,
                    PushNotifications = false,
                };

                IdentityResult result = await userManager.CreateAsync(user: newUser, password: user.Password!);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user: newUser, isPersistent: false);

                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(key: "", errorMessage: error.Description);
                }
            }

            return View(model: user);
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
                User? user = await userManager.FindByEmailAsync(email: viewModel.Email!);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user: user)))
                {
                    return RedirectToAction(actionName: "ForgotPasswordConfirmation", controllerName: "Account");
                }

                string code = await userManager.GeneratePasswordResetTokenAsync(user: user);
                code = WebEncoders.Base64UrlEncode(input: Encoding.UTF8.GetBytes(s: code));
                string? callbackUrl = Url.Page(
                pageName: "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme
                );

                await emailSender.SendEmailAsync(
                    email: viewModel.Email!,
                    subject: "Khôi phục mật khẩu",
                    htmlMessage: $"Để khôi phục mật khẩu của bạn, vui lòng <a href='{HtmlEncoder.Default.Encode(value: callbackUrl!)}'>nhấp vào đây</a>."
                );

                return RedirectToAction(actionName: "ForgotPasswordConfirmation", controllerName: "Account");
            }

            return View(model: viewModel);
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
            return View(model: viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyAuthenticatorCode(bool rememberMe, string? returnUrl = null)
        {
            User? user = await signInManager.GetTwoFactorAuthenticationUserAsync();
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
            string? redirectUrl = Url.Action(action: "ExternalLoginCallback", controller: "Account", values: new { returnurl });
            AuthenticationProperties properties = signInManager.ConfigureExternalAuthenticationProperties(provider: provider, redirectUrl: redirectUrl);
            return Challenge(properties: properties, authenticationSchemes: provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnurl = null, string? remoteError = null)
        {
            returnurl ??= Url.Content(contentPath: "~/");
            if (remoteError != null)
            {
                ModelState.AddModelError(key: string.Empty, errorMessage: $"Error from external provider: {remoteError}");
                return View(viewName: nameof(Login));
            }

            ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(actionName: nameof(Login));
            }

            SignInResult result = await signInManager.ExternalLoginSignInAsync(
                loginProvider: info.LoginProvider,
                providerKey: info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true
            );

            if (result.Succeeded)
            {
                await signInManager.UpdateExternalAuthenticationTokensAsync(externalLogin: info);
                return LocalRedirect(localUrl: returnurl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(actionName: nameof(VerifyAuthenticatorCode), routeValues: new { returnurl });
            }

            ViewData[index: "ReturnUrl"] = returnurl;
            ViewData[index: "ProviderDisplayName"] = info.ProviderDisplayName;

            return View(viewName: "ExternalLoginConfirmation", model: new ExternalLoginConfirmationViewModel
            {
                Email = info.Principal.FindFirstValue(claimType: ClaimTypes.Email),
                Name = info.Principal.FindFirstValue(claimType: ClaimTypes.Name)
            });
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
                ExternalLoginInfo? info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return NotFound();
                }

                User user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Email,
                    EmailConfirmed = true,
                    ShowEmail = false,
                    AllowChat = false,
                    EmailNotifications = false,
                    PushNotifications = false,
                };

                IdentityResult result = await userManager.CreateAsync(user: user, password: model.Password!);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user: user, login: info);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user: user, isPersistent: false);
                        await signInManager.UpdateExternalAuthenticationTokensAsync(externalLogin: info);
                        return LocalRedirect(localUrl: returnurl);
                    }
                }
                AddErrors(result: result);
            }
            ViewData[index: "ReturnUrl"] = returnurl;
            return View(model: model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);
            }
        }
    }
}
