using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.Models
{
	public class User : IdentityUser
	{
		[Required]
		public string? Name { get; set; }
	}
}