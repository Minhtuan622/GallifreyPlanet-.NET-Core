using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class AccountManagerController(
        UserService userService,
        FileService fileService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            AccountManagerViewModel viewModel = new AccountManagerViewModel
            {
                User = user,
                LoginHistory = await userService.GetLoginHistoriesAsyncByUserId(user.Id),
                ActiveSessions = await userService.GetActiveSessionsAsyncByUser(user.Id),
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AccountSetting()
        {
            User? user = await userService.GetCurrentUserAsync();
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
                    AllowMessagesFromNonFriends = user.AllowChat,
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

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.ChangePasswordAsync(
                user,
                viewModel.ChangePassword!.CurrentPassword!,
                viewModel.ChangePassword!.NewPassword!
            );

            if (result.Succeeded)
            {
                TempData[key: "StatusMessage"] = "Đổi mật khẩu thành công";
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

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.UpdateProfileAsync(user, viewModel.EditProfile!);

            if (viewModel.EditProfile!.AvatarFile != null)
            {
                user.Avatar = await fileService.UploadFileAsync(
                    viewModel.EditProfile.AvatarFile,
                    folder: "/accounts/avatars",
                    user.Avatar!
                );
                await userService.UpdateProfileAsync(user, viewModel.EditProfile);
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

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.UpdatePrivacySettingsAsync(user, viewModel.PrivacySettings!);

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

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.UpdateNotificationSettingsAsync(user, viewModel.NotificationSettings!);

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
            if (avatar.Length == 0)
            {
                return Json(new { success = false });
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Json(new { success = false });
            }

            string avatarPath = await fileService.UploadFileAsync(avatar, folder: "/accounts/avatars", user.Avatar!);
            user.Avatar = avatarPath;
            IdentityResult result = await userService.UpdateProfileAsync(user, model: new EditProfileViewModel { CurrentAvatar = avatarPath });

            return Json(new { success = result.Succeeded, avatarUrl = user.Avatar });
        }

        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            string token = await userService.GenerateTwoFactorTokenAsync(user);
            EnableTwoFactorAuthenticationViewModel model = new EnableTwoFactorAuthenticationViewModel { Token = token };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            string verificationCode = model.VerificationCode!.Replace(oldValue: " ", string.Empty).Replace(oldValue: "-", string.Empty);
            bool is2FaTokenValid = await userService.VerifyTwoFactorTokenAsync(user, verificationCode);

            if (!is2FaTokenValid)
            {
                ModelState.AddModelError(key: "VerificationCode", errorMessage: "Mã xác thực không hợp lệ.");
                return View(model);
            }

            await userService.SetTwoFactorEnabledAsync(user, enabled: true);
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
            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            await userService.TerminateSessionAsync(user.Id, sessionId);
            return RedirectToAction(nameof(Index));
        }
    }
}
