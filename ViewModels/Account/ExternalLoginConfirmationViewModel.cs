using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Account;

public class ExternalLoginConfirmationViewModel
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress]
    public string? Email { get; set; }

    [Display(Name = "Họ và tên", Prompt = "Họ và tên là bắt buộc")]
    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    public string? Name { get; set; }

    [Display(Name = "Mật khẩu", Prompt = "Mật khẩu là bắt buộc")]
    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(maximumLength: 100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
    public string? Password { get; set; }

    [Display(Name = "Nhập lại mật khẩu", Prompt = "Mật khẩu xác nhận là bắt buộc")]
    [Compare(otherProperty: "Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
    [Required(ErrorMessage = "Chưa xác nhận mật khẩu")]
    public string? ConfirmPassword { get; set; }
}