using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Account
{
	public class ResetPasswordViewModel
	{
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
