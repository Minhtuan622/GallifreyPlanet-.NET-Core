using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.AccountManager
{
    public class AccountManagerViewModel
    {
        public User? User { get; set; }
        public AccountInfoViewModel? AccountInfo { get; set; }
        public ChangePasswordViewModel? ChangePassword { get; set; }
        public EditProfileViewModel? EditProfile { get; set; }
        public PrivacySettingsViewModel? PrivacySettings { get; set; }
        public NotificationSettingsViewModel? NotificationSettings { get; set; }
        public List<LoginHistoryViewModel> LoginHistory { get; set; } = new List<LoginHistoryViewModel>();
        public List<ActiveSessionViewModel> ActiveSessions { get; set; } = new List<ActiveSessionViewModel>();
        public List<string> LinkedAccounts { get; set; } = new List<string>();
        public LoginHistoryViewModel? LoginHistoryViewModel { get; set; }
        public ActiveSessionViewModel? ActiveSessionViewModel { get; set; }
    }
}