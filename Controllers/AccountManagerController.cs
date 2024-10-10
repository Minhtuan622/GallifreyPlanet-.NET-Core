using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class AccountManagerController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountManagerController(
            UserManager<User> userManager,
            IWebHostEnvironment webHostEnvironment
        )
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            User? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> AccountSetting()
        {
            User? user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            AccountManagerViewModel viewModel = new AccountManagerViewModel
            {
                ChangePassword = new ChangePasswordViewModel(),
                EditProfile = new EditProfileViewModel
                {
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
            if (ModelState.IsValid)
            {
                User? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                IdentityResult? result = await _userManager.ChangePasswordAsync(
                    user,
                    viewModel.ChangePassword!.CurrentPassword!,
                    viewModel.ChangePassword!.NewPassword!
                );
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(key: "", error.Description);
                }
            }
            return RedirectToAction(nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(AccountManagerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = viewModel.EditProfile!.Name;
                user.Email = viewModel.EditProfile!.Email;
                user.Address = viewModel.EditProfile!.Address;

                if (viewModel.EditProfile.AvatarFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.EditProfile!.AvatarFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.EditProfile!.AvatarFile.CopyToAsync(fileStream);
                    }

                    user.Avatar = "/uploads/" + uniqueFileName;
                }

                IdentityResult? result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(key: "", error.Description);
                }
            }
            return RedirectToAction(nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> PrivacySettings(AccountManagerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.ShowEmail = viewModel.PrivacySettings!.ShowEmail;
                user.AllowMessagesFromNonFriends = viewModel.PrivacySettings!.AllowMessagesFromNonFriends;

                IdentityResult? result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(key: "", error.Description);
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountSetting));
        }

        [HttpPost]
        public async Task<IActionResult> NotificationSettings(AccountManagerViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.EmailNotifications = viewModel.NotificationSettings!.EmailNotifications;
                user.PushNotifications = viewModel.NotificationSettings!.PushNotifications;

                IdentityResult? result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(key: "", error.Description);
                }

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AccountSetting));
        }
    }
}
