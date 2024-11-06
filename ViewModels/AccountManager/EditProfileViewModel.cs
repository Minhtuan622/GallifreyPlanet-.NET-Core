using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.AccountManager;

public class EditProfileViewModel
{
    [Display(Name = "Tên người dùng")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [Display(Name = "Họ và tên", Prompt = "Nhập họ và tên")]
    public string? Name { get; set; }

    [Display(Name = "Số điện thoại", Prompt = "Nhập số điện thoại")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email", Prompt = "Nhập Email")]
    public string? Email { get; set; }

    [Display(Name = "Địa chỉ", Prompt = "Nhập địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Ảnh đại diện")]
    public IFormFile? AvatarFile { get; set; }

    public string? CurrentAvatar { get; set; }
}