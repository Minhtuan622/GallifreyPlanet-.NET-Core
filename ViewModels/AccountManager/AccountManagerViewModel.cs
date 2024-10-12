using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.AccountManager
{
    public class AccountManagerViewModel
    {
        public User? User { get; set; }
        public ChangePasswordViewModel? ChangePassword { get; set; }
        public EditProfileViewModel? EditProfile { get; set; }
        public PrivacySettingsViewModel? PrivacySettings { get; set; }
        public NotificationSettingsViewModel? NotificationSettings { get; set; }
        public List<LoginHistory>? LoginHistory { get; set; }
        public List<ActiveSessionViewModel>? ActiveSessions { get; set; }
        public List<string> LinkedAccounts { get; set; } = new List<string>();
    }
}
