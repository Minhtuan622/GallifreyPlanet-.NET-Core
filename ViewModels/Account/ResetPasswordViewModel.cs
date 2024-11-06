using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Account;

public class ResetPasswordViewModel
{
    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(maximumLength: 100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
    public string? Password { get; set; }


    [Display(Name = "Xác nhận mật khẩu")]
    [Compare(otherProperty: "Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
    [Required(ErrorMessage = "Chưa xác nhận mật khẩu")]
    public string? ConfirmPassword { get; set; }
}