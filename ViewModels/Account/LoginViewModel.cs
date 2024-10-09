using System.ComponentModel.DataAnnotations;


namespace GallifreyPlanet.ViewModels.Account
{

	public class LoginViewModel
	{
		[Display(Name = "Tên người dùng hoặc Email")]
		[Required(ErrorMessage = "Tên người dùng hoặc Email là bắt buộc.")]
		public string? UsernameOrEmail { get; set; }

		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[Display(Name = "Duy trì đăng nhập")]
		public bool RememberMe { get; set; }
	}
}
