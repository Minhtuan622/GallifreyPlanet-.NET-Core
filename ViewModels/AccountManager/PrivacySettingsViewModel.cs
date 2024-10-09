using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.AccountManager
{
    public class PrivacySettingsViewModel
    {
        [Display(Name = "Hiển thị email cho người khác")]
        public bool ShowEmail { get; set; }

        [Display(Name = "Cho phép tin nhắn từ người không phải bạn bè")]
        public bool AllowMessagesFromNonFriends { get; set; }
    }
}