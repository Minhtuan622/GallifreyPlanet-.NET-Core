using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.Models.Auth
{
	[Table("users")]
	public class LoginViewModel
	{
		[Column("phone")]
		[Display(Name = "Số điện thoại")]
		[Required(ErrorMessage = "Số điện thoại là bắt buộc")]
		[StringLength(11, MinimumLength = 9, ErrorMessage = "Số điện thoại phải từ 9 đến 11 ký tự")]
		public string? Phone { get; set; }

		[Column("password")]
		[DataType(DataType.Password)]
		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Mật khẩu là bắt buộc")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
		public string? Password { get; set; }
	}
}
