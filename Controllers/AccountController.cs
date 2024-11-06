using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GallifreyPlanet.Controllers;

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
    public bool IsValidEmail(string emailAddress)
    {
        try
        {
            var m = new MailAddress(address: emailAddress);
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
        if (!ModelState.IsValid)
        {
            return View(model: user);
        }
            
        var userNameOrEmail = user.UsernameOrEmail;

        if (IsValidEmail(emailAddress: user.UsernameOrEmail!))
        {
            var getUser = await userManager.FindByEmailAsync(email: user.UsernameOrEmail!);
            if (getUser != null)
            {
                userNameOrEmail = getUser.UserName;
            }
        }

        var result = await signInManager.PasswordSignInAsync(
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
            var currentUser = await userService.GetCurrentUserAsync();
            await userService.AddLoginHistoryAsync(
                userId: currentUser!.Id,
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString()!,
                userAgent: HttpContext.Request.Headers.UserAgent.ToString()
            );

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        ModelState.AddModelError(key: "", errorMessage: "Vui lòng kiểm tra tên người dùng hoặc mật khẩu.");

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
        if (!ModelState.IsValid)
        {
            return View(model: user);
        }
            
        var existingUser = context.Users.FirstOrDefault(predicate: u => u.Email == user.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError(key: "", errorMessage: "Email đã tồn tại.");
            return View(model: user);
        }

        var newUser = new User
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

        var result = await userManager.CreateAsync(user: newUser, password: user.Password!);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user: newUser, isPersistent: false);

            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(key: "", errorMessage: error.Description);
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
        if (!ModelState.IsValid)
        {
            return View(model: viewModel);
        }
            
        var user = await userManager.FindByEmailAsync(email: viewModel.Email!);
        if (user == null || !(await userManager.IsEmailConfirmedAsync(user: user)))
        {
            return RedirectToAction(actionName: "ForgotPasswordConfirmation", controllerName: "Account");
        }

        var code = await userManager.GeneratePasswordResetTokenAsync(user: user);
        code = WebEncoders.Base64UrlEncode(input: Encoding.UTF8.GetBytes(s: code));
        var callbackUrl = Url.Page(
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
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
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
        var redirectUrl = Url.Action(action: "ExternalLoginCallback", controller: "Account", values: new { returnurl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider: provider, redirectUrl: redirectUrl);
        return Challenge(properties: properties, authenticationSchemes: provider);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content(contentPath: "~/");
        if (remoteError != null)
        {
            ModelState.AddModelError(key: string.Empty, errorMessage: $"Error from external provider: {remoteError}");
            return View(viewName: nameof(Login));
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(actionName: nameof(Login));
        }

        var result = await signInManager.ExternalLoginSignInAsync(
            loginProvider: info.LoginProvider,
            providerKey: info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true
        );

        if (result.Succeeded)
        {
            await signInManager.UpdateExternalAuthenticationTokensAsync(externalLogin: info);
            return LocalRedirect(localUrl: returnUrl);
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectToAction(actionName: nameof(VerifyAuthenticatorCode), routeValues: new { returnurl = returnUrl });
        }

        ViewData[index: "ReturnUrl"] = returnUrl;
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
        string? returnUrl = null
    )
    {
        returnUrl ??= Url.Content(contentPath: "~/");

        if (ModelState.IsValid)
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return NotFound();
            }

            var user = new User
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

            var result = await userManager.CreateAsync(user: user, password: model.Password!);
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user: user, login: info);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user: user, isPersistent: false);
                    await signInManager.UpdateExternalAuthenticationTokensAsync(externalLogin: info);
                    return LocalRedirect(localUrl: returnUrl);
                }
            }
            AddErrors(result: result);
        }
        ViewData[index: "ReturnUrl"] = returnUrl;
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
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);
        }
    }
}