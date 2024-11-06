using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.AccountManager;

public class NotificationSettingsViewModel
{
    [Display(Name = "Nhận thông báo qua email")]
    public bool EmailNotifications { get; set; }

    [Display(Name = "Nhận thông báo đẩy")]
    public bool PushNotifications { get; set; }
}