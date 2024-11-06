using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.AccountManager;

public class PrivacySettingsViewModel
{
    [Display(Name = "Hiển thị email công khai")]
    public bool ShowEmail { get; set; }

    [Display(Name = "Cho phép tin nhắn từ người lạ")]
    public bool AllowMessagesFromNonFriends { get; set; }

    [Display(Name = "Cho phép người lạ kết bạn")]
    public bool AllowAddFriend { get; set; }
}