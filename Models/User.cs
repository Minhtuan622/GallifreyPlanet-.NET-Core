using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
    }
}