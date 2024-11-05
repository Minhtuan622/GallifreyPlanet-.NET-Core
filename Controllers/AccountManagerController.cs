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
                LoginHistory = await userService.GetLoginHistoriesAsyncByUserId(userId: user.Id),
                ActiveSessions = await userService.GetActiveSessionsAsyncByUser(userId: user.Id),
            };

            return View(model: viewModel);
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

            return View(model: viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(actionName: nameof(AccountSetting), routeValues: viewModel);
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.ChangePasswordAsync(
                user: user,
                currentPassword: viewModel.ChangePassword!.CurrentPassword!,
                newPassword: viewModel.ChangePassword!.NewPassword!
            );

            if (result.Succeeded)
            {
                TempData[key: "StatusMessage"] = "Đổi mật khẩu thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            AddErrors(result: result);
            return RedirectToAction(actionName: nameof(AccountSetting), routeValues: viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(actionName: nameof(AccountSetting));
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.UpdateProfileAsync(user: user, model: viewModel.EditProfile!);

            if (viewModel.EditProfile!.AvatarFile != null)
            {
                user.Avatar = await fileService.UploadFileAsync(
                    file: viewModel.EditProfile.AvatarFile,
                    folder: "/accounts/avatars",
                    currentFilePath: user.Avatar!
                );
                await userService.UpdateProfileAsync(user: user, model: viewModel.EditProfile);
            }

            if (result.Succeeded)
            {
                TempData[key: "StatusMessage"] = "Cập nhật thông tin cá nhân thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            AddErrors(result: result);
            return RedirectToAction(actionName: nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> PrivacySettings(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(actionName: nameof(AccountSetting));
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.UpdatePrivacySettingsAsync(user: user, model: viewModel.PrivacySettings!);

            if (result.Succeeded)
            {
                TempData[key: "StatusMessage"] = "Cập nhật cài đặt quyền riêng tư thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            AddErrors(result: result);
            return RedirectToAction(actionName: nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> NotificationSettings(AccountManagerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(actionName: nameof(AccountSetting));
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await userService.UpdateNotificationSettingsAsync(user: user, model: viewModel.NotificationSettings!);

            if (result.Succeeded)
            {
                TempData[key: "StatusMessage"] = "Cập nhật cài đặt thông báo thành công";
                return RedirectToAction(actionName: nameof(Index));
            }

            AddErrors(result: result);
            return RedirectToAction(actionName: nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            if (avatar.Length == 0)
            {
                return Json(data: new { success = false });
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return Json(data: new { success = false });
            }

            string avatarPath = await fileService.UploadFileAsync(file: avatar, folder: "/accounts/avatars", currentFilePath: user.Avatar!);
            user.Avatar = avatarPath;
            IdentityResult result = await userService.UpdateProfileAsync(user: user, model: new EditProfileViewModel { CurrentAvatar = avatarPath });

            return Json(data: new { success = result.Succeeded, avatarUrl = user.Avatar });
        }

        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            string token = await userService.GenerateTwoFactorTokenAsync(user: user);
            EnableTwoFactorAuthenticationViewModel model = new EnableTwoFactorAuthenticationViewModel { Token = token };

            return View(model: model);
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model: model);
            }

            User? user = await userService.GetCurrentUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            string verificationCode = model.VerificationCode!.Replace(oldValue: " ", newValue: string.Empty).Replace(oldValue: "-", newValue: string.Empty);
            bool is2FaTokenValid = await userService.VerifyTwoFactorTokenAsync(user: user, verificationCode: verificationCode);

            if (!is2FaTokenValid)
            {
                ModelState.AddModelError(key: "VerificationCode", errorMessage: "Mã xác thực không hợp lệ.");
                return View(model: model);
            }

            await userService.SetTwoFactorEnabledAsync(user: user, enabled: true);
            return RedirectToAction(actionName: nameof(Index));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError? error in result.Errors)
            {
                ModelState.AddModelError(key: string.Empty, errorMessage: error.Description);
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

            await userService.TerminateSessionAsync(userId: user.Id, sessionId: sessionId);
            return RedirectToAction(actionName: nameof(Index));
        }
    }
}
