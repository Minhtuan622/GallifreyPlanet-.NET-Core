using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Account
{

    public class SignupViewModel
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(maximumLength: 100, ErrorMessage = "Họ và tên không được quá 100 ký tự")]
        public string? Name { get; set; }

        [Display(Name = "Tên người dùng")]
        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        public string? UserName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        public string? Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(maximumLength: 100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
        public string? Password { get; set; }


        [Display(Name = "Xác nhận mật khẩu")]
        [Compare(otherProperty: "Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
        [Required(ErrorMessage = "Chưa xác nhận mật khẩu")]
        public string? ConfirmPassword { get; set; }
    }
}
