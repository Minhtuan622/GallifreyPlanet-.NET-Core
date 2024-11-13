using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.AccountManager;

public class SocialMediaViewModel
{
    [Display(Name = "Website", Prompt = "Địa chỉ trang web cá nhân")]
    public string? PersonalWebsite { get; set; }
    [Display(Prompt = "Tên người dùng Facebook")]
    public string? Facebook { get; set; }
    [Display(Prompt = "Tên người dùng Github")]
    public string? Github { get; set; }
    [Display(Prompt = "Tên người dùng Twitter")]
    public string? Twitter { get; set; }
    [Display(Prompt = "Tên người dùng Instagram")]
    public string? Instagram { get; set; }
}