using System.ComponentModel.DataAnnotations;


namespace GallifreyPlanet.ViewModels.Auth
{

	public class LoginViewModel
	{
		[Display(Name = "Tên người dùng")]
		[Required(ErrorMessage = "Tên người dùng là bắt buộc")]
		public string? Username { get; set; }

		[Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
	}
}
