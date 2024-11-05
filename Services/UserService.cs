using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.AccountManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services
{
    public class UserService(
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        GallifreyPlanetContext context)
    {
        public async Task<User?> GetCurrentUserAsync()
        {
            return await userManager.GetUserAsync(principal: httpContextAccessor.HttpContext!.User);
        }

        public async Task<User?> GetUserAsyncByUserName(string username)
        {
            return await userManager.FindByNameAsync(userName: username);
        }

        public async Task<List<User>?> GetUsersAsync()
        {
            return await context.User.ToListAsync();
        }

        public async Task<User?> GetUserAsyncById(string userId)
        {
            return await userManager.FindByIdAsync(userId: userId);
        }

        public async Task<List<ActiveSessionViewModel>> GetActiveSessionsAsyncByUser(string userId)
        {
            return await context.UserSession
                .Where(predicate: us => us.UserId == userId && us.LogoutTime == null)
                .Select(selector: us => new ActiveSessionViewModel
                {
                    Id = us.Id.ToString(),
                    DeviceName = us.DeviceName,
                    Location = us.Location,
                    LoginTime = us.LoginTime
                })
                .ToListAsync();
        }

        public async Task<List<LoginHistory>> GetLoginHistoriesAsyncByUserId(string userId)
        {
            return await context.LoginHistory
                .Where(predicate: lh => lh.UserId == userId)
                .OrderByDescending(keySelector: lh => lh.LoginTime)
                .Take(count: 10)
                .ToListAsync();
        }

        public async Task<IdentityResult> UpdateProfileAsync(User user, EditProfileViewModel model)
        {
            user.Name = model.Name;
            user.Email = model.Email;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            return await userManager.UpdateAsync(user: user);
        }

        public async Task<IdentityResult> UpdatePrivacySettingsAsync(User user, PrivacySettingsViewModel model)
        {
            user.ShowEmail = model.ShowEmail;
            user.AllowChat = model.AllowMessagesFromNonFriends;
            user.AllowAddFriend = model.AllowAddFriend;
            return await userManager.UpdateAsync(user: user);
        }

        public async Task<IdentityResult> UpdateNotificationSettingsAsync(User user, NotificationSettingsViewModel model)
        {
            user.EmailNotifications = model.EmailNotifications;
            user.PushNotifications = model.PushNotifications;
            return await userManager.UpdateAsync(user: user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            return await userManager.ChangePasswordAsync(user: user, currentPassword: currentPassword, newPassword: newPassword);
        }

        public async Task<string> GenerateTwoFactorTokenAsync(User user)
        {
            return await userManager.GenerateTwoFactorTokenAsync(user: user, tokenProvider: "Authenticator");
        }

        public async Task<bool> VerifyTwoFactorTokenAsync(User user, string verificationCode)
        {
            return await userManager.VerifyTwoFactorTokenAsync(user: user, tokenProvider: "Authenticator", token: verificationCode);
        }

        public async Task<IdentityResult> SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            return await userManager.SetTwoFactorEnabledAsync(user: user, enabled: enabled);
        }

        public async Task TerminateSessionAsync(string userId, string sessionId)
        {
            UserSession? session = await context.UserSession
                .FirstOrDefaultAsync(predicate: us => us.Id.ToString() == sessionId && us.UserId == userId);

            if (session != null)
            {
                session.LogoutTime = DateTime.UtcNow;
                await context.SaveChangesAsync();
            }
        }

        public async Task AddLoginHistoryAsync(string userId, string ipAddress, string userAgent)
        {
            LoginHistory loginHistory = new LoginHistory
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            context.LoginHistory.Add(entity: loginHistory);
            await context.SaveChangesAsync();
        }
    }
}
