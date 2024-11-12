using Microsoft.AspNetCore.Identity;

namespace GallifreyPlanet.Models;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public string? Address { get; set; }
    public string? PersonalWebsite { get; set; }
    public string? Facebook { get; set; }
    public string? Github { get; set; }
    public string? Twitter { get; set; }
    public string? Instagram { get; set; }
    public bool ShowEmail { get; set; }
    public bool AllowChat { get; set; }
    public bool AllowAddFriend { get; set; }
    public bool EmailNotifications { get; set; }
    public bool PushNotifications { get; set; }
}