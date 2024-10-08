using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.Models
{
	public class User : IdentityUser
	{
		[Required(ErrorMessage = "Họ và tên là bắt buộc")]
		public string? Name { get; set; }
	}
}