using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.Models.Auth
{
	[Table("users")]
	public class SignupViewModel
	{

		[Column("id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Column("name")]
		[Display(Name = "Họ và tên")]
		[Required(ErrorMessage = "Họ và tên là bắt buộc")]
		[StringLength(100, ErrorMessage = "Họ và tên không được quá 100 ký tự")]
		public string? Name { get; set; }

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

		[NotMapped]
		[Display(Name = "Xác nhận mật khẩu")]
		[Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp")]
		[Required(ErrorMessage = "Chưa xác nhận mật khẩu")]
		public string? ConfirmPassword { get; set; }
	}
}
