using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GallifreyPlanetContext _gallifreyPlanetContext;

        public UserService(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            GallifreyPlanetContext gallifreyPlanetContext
        )
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _gallifreyPlanetContext = gallifreyPlanetContext;
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
        }

        public async Task<List<ActiveSessionViewModel>> GetActiveSessionsAsync(User user)
        {
            return await _gallifreyPlanetContext.UserSession
                .Where(us => us.UserId == user.Id && us.LogoutTime == null)
                .Select(us => new ActiveSessionViewModel
                {
                    Id = us.Id.ToString(),
                    DeviceName = us.DeviceName,
                    Location = us.Location,
                    LoginTime = us.LoginTime
                })
                .ToListAsync();
        }

        public async Task<List<LoginHistory>> GetLoginHistoriesAsync(User user)
        {
            return await _gallifreyPlanetContext.LoginHistory
                .Where(lh => lh.UserId == user.Id)
                .OrderByDescending(lh => lh.LoginTime)
                .Take(count: 10)
                .ToListAsync();
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

        public async Task TerminateSessionAsync(string userId, string sessionId)
        {
            UserSession? session = await _gallifreyPlanetContext.UserSession
                .FirstOrDefaultAsync(us => us.Id.ToString() == sessionId && us.UserId == userId);

            if (session != null)
            {
                session.LogoutTime = DateTime.UtcNow;
                await _gallifreyPlanetContext.SaveChangesAsync();
            }
        }

        public async Task AddLoginHistoryAsync(string userId, string ipAddress, string userAgent)
        {
            LoginHistory? loginHistory = new LoginHistory
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _gallifreyPlanetContext.LoginHistory.Add(loginHistory);
            await _gallifreyPlanetContext.SaveChangesAsync();
        }
    }
}
