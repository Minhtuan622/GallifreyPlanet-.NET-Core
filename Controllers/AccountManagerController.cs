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

        public AccountManagerController(UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
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
                AccountInfo = new AccountInfoViewModel
                {
                    Name = user.Name!,
                    Username = user.UserName!,
                    Email = user.Email!,
                    Avatar = user.Avatar
                },
                ChangePassword = new ChangePasswordViewModel(),
                EditProfile = new EditProfileViewModel
                {
                    Name = user.Name,
                    Email = user.Email,
                    CurrentAvatar = user.Avatar
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                IdentityResult? result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);
                if (result.Succeeded)
                {
                    // Password changed successfully
                    return RedirectToAction(nameof(Index));
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                user.Name = model.Name;
                user.Email = model.Email;

                if (model.AvatarFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.AvatarFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.AvatarFile.CopyToAsync(fileStream);
                    }

                    user.Avatar = "/uploads/" + uniqueFileName;
                }

                IdentityResult? result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Profile updated successfully
                    return RedirectToAction(nameof(Index));
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult PrivacySettings()
        {
            // Load current privacy settings
            PrivacySettingsViewModel? model = new PrivacySettingsViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult PrivacySettings(PrivacySettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save privacy settings
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult NotificationSettings()
        {
            // Load current notification settings
            NotificationSettingsViewModel? model = new NotificationSettingsViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult NotificationSettings(NotificationSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save notification settings
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
