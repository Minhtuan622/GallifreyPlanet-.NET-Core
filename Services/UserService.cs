using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;

namespace GallifreyPlanet.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
        }

        public EditProfileViewModel CreateEditProfileViewModel(User user)
        {
            return new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                CurrentAvatar = user.Avatar
            };
        }

        public PrivacySettingsViewModel CreatePrivacySettingsViewModel(User user)
        {
            return new PrivacySettingsViewModel
            {
                ShowEmail = user.ShowEmail,
                AllowMessagesFromNonFriends = user.AllowMessagesFromNonFriends,
            };
        }

        public NotificationSettingsViewModel CreateNotificationSettingsViewModel(User user)
        {
            return new NotificationSettingsViewModel
            {
                EmailNotifications = user.EmailNotifications,
                PushNotifications = user.PushNotifications,
            };
        }

        public PublicProfileViewModel CreatePublicProfileViewModel(User user)
        {
            return new PublicProfileViewModel
            {
                UserName = user.UserName!,
                Name = user.Name!,
                Avatar = user.Avatar!,
                Email = user.ShowEmail ? user.Email : null
            };
        }

        public async Task<IdentityResult> UpdateProfileAsync(User user, EditProfileViewModel model)
        {
            user.Name = model.Name;
            user.Email = model.Email;
            user.Address = model.Address;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdatePrivacySettingsAsync(User user, PrivacySettingsViewModel model)
        {
            user.ShowEmail = model.ShowEmail;
            user.AllowMessagesFromNonFriends = model.AllowMessagesFromNonFriends;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateNotificationSettingsAsync(User user, NotificationSettingsViewModel model)
        {
            user.EmailNotifications = model.EmailNotifications;
            user.PushNotifications = model.PushNotifications;
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<string> GenerateTwoFactorTokenAsync(User user)
        {
            return await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider: "Authenticator");
        }

        public async Task<bool> VerifyTwoFactorTokenAsync(User user, string verificationCode)
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, tokenProvider: "Authenticator", verificationCode);
        }

        public async Task<IdentityResult> SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            return await _userManager.SetTwoFactorEnabledAsync(user, enabled);
        }
    }
}
