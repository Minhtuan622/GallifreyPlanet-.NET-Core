using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class AccountManagerController : Controller
    {
        private readonly UserService _userService;
        private readonly FileService _fileService;
        private readonly UserManager<User> _userManager;
        private readonly GallifreyPlanetContext _context;

        public AccountManagerController(
            UserService userService,
            FileService fileService,
            IWebHostEnvironment webHostEnvironment,
            UserManager<User> userManager,
            GallifreyPlanetContext context
        )
        {
            _userService = userService;
            _fileService = fileService;
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
                LoginHistory = await _userService.GetLoginHistoriesAsyncByUserId(user.Id),
                ActiveSessions = await _userService.GetActiveSessionsAsyncByUser(user.Id),
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
                EditProfile = new EditProfileViewModel
                {
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    CurrentAvatar = user.Avatar
                },
                PrivacySettings = new PrivacySettingsViewModel
                {
                    ShowEmail = user.ShowEmail,
                    AllowMessagesFromNonFriends = user.AllowMessagesFromNonFriends,
                },
                NotificationSettings = new NotificationSettingsViewModel
                {
                    EmailNotifications = user.EmailNotifications,
                    PushNotifications = user.PushNotifications,
                },
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
                TempData[key: "StatusMessage"] = "Cập nhật mật khẩu thành công";
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
                user.Avatar = await _fileService.UploadFileAsync(viewModel.EditProfile.AvatarFile, uploadFolder: "/accounts/avatars");
                await _userService.UpdateProfileAsync(user, viewModel.EditProfile);
            }

            if (result.Succeeded)
            {
                TempData[key: "StatusMessage"] = "Cập nhật thông tin cá nhân thành công";
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
                TempData[key: "StatusMessage"] = "Cập nhật cài đặt quyền riêng tư thành công";
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
                TempData[key: "StatusMessage"] = "Cập nhật cài đặt thông báo thành công";
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

            string? avatarPath = await _fileService.UploadFileAsync(avatar, uploadFolder: "/accounts/avatars");
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

            string verificationCode = model.VerificationCode!.Replace(oldValue: " ", string.Empty).Replace(oldValue: "-", string.Empty);
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
