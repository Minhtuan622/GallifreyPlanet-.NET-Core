using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Account
{

    public class SignupViewModel
    {
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ và tên không được quá 100 ký tự")]
        public string? Name { get; set; }

        [Display(Name = "Tên người dùng")]
        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        public string? UserName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        public string? Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]        
        public string? Password { get; set; }


        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
        [Required(ErrorMessage = "Chưa xác nhận mật khẩu")]
        public string? ConfirmPassword { get; set; }
    }
}
