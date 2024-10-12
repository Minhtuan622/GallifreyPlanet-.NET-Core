using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class AccountManagerController : Controller
    {
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly GallifreyPlanetContext _context;

        public AccountManagerController(
            UserService userService,
            IWebHostEnvironment webHostEnvironment,
            UserManager<User> userManager,
            GallifreyPlanetContext context
        )
        {
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            AccountManagerViewModel? viewModel = new AccountManagerViewModel
            {
                User = user,
                LoginHistory = await _userService.GetLoginHistoriesAsync(user),
                ActiveSessions = await _userService.GetActiveSessionsAsync(user),
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AccountSetting()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            AccountManagerViewModel viewModel = new AccountManagerViewModel
            {
                ChangePassword = new ChangePasswordViewModel(),
                EditProfile = _userService.CreateEditProfileViewModel(user),
                PrivacySettings = _userService.CreatePrivacySettingsViewModel(user),
                NotificationSettings = _userService.CreateNotificationSettingsViewModel(user),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AccountSetting), viewModel);
            }

            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult? result = await _userService.ChangePasswordAsync(
                user,
                viewModel.ChangePassword!.CurrentPassword!,
                viewModel.ChangePassword!.NewPassword!
            );

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            AddErrors(result);
            return RedirectToAction(nameof(AccountSetting), viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AccountSetting));
            }

            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult? result = await _userService.UpdateProfileAsync(user, viewModel.EditProfile!);

            if (viewModel.EditProfile!.AvatarFile != null)
            {
                user.Avatar = await UploadAvatarAsync(viewModel.EditProfile.AvatarFile);
                await _userService.UpdateProfileAsync(user, viewModel.EditProfile);
            }

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            AddErrors(result);
            return RedirectToAction(nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> PrivacySettings(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AccountSetting));
            }

            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult? result = await _userService.UpdatePrivacySettingsAsync(user, viewModel.PrivacySettings!);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            AddErrors(result);
            return RedirectToAction(nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> NotificationSettings(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(AccountSetting));
            }

            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult? result = await _userService.UpdateNotificationSettingsAsync(user, viewModel.NotificationSettings!);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            AddErrors(result);
            return RedirectToAction(nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
            {
                return Json(new { success = false });
            }

            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Json(new { success = false });
            }

            string? avatarPath = await UploadAvatarAsync(avatar);
            user.Avatar = avatarPath;
            IdentityResult? result = await _userService.UpdateProfileAsync(user, model: new EditProfileViewModel { CurrentAvatar = avatarPath });

            return Json(new { success = result.Succeeded, avatarUrl = user.Avatar });
        }

        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            string? token = await _userService.GenerateTwoFactorTokenAsync(user);
            EnableTwoFactorAuthenticationViewModel? model = new EnableTwoFactorAuthenticationViewModel { Token = token };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            string verificationCode = model.VerificationCode.Replace(oldValue: " ", string.Empty).Replace(oldValue: "-", string.Empty);
            bool is2faTokenValid = await _userService.VerifyTwoFactorTokenAsync(user, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError(key: "VerificationCode", errorMessage: "Mã xác thực không hợp lệ.");
                return View(model);
            }

            await _userService.SetTwoFactorEnabledAsync(user, enabled: true);
            return RedirectToAction(nameof(Index));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError? error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<string> UploadAvatarAsync(IFormFile avatarFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(fileStream);
            }

            return "/uploads/" + uniqueFileName;
        }

        [HttpPost]
        public async Task<IActionResult> TerminateSession(string sessionId)
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            await _userService.TerminateSessionAsync(user.Id, sessionId);
            return RedirectToAction(nameof(Index));
        }
    }
}
