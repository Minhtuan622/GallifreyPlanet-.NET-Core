using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models
{
	[Table("Users")]
	public class User : IdentityUser
	{
	}
}