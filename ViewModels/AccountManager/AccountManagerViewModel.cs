namespace GallifreyPlanet.ViewModels.AccountManager
{
    public class AccountManagerViewModel
    {
        public AccountInfoViewModel? AccountInfo { get; set; }
        public ChangePasswordViewModel? ChangePassword { get; set; }
        public EditProfileViewModel? EditProfile { get; set; }
        public PrivacySettingsViewModel? PrivacySettings { get; set; }
        public NotificationSettingsViewModel? NotificationSettings { get; set; }
    }
}