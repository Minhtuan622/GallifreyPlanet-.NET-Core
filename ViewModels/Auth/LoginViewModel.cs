using System.ComponentModel.DataAnnotations;


namespace GallifreyPlanet.ViewModels.Auth
{

	public class LoginViewModel
	{
		[Display(Name = "Email")]
		[Required(ErrorMessage = "Email là bắt buộc")]
		[StringLength(100, MinimumLength = 9, ErrorMessage = "Số điện thoại phải từ 9 đến 11 ký tự")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
	}
}
