namespace GallifreyPlanet.ViewModels.AccountManager;

public class LoginHistoryViewModel
{
    public DateTime LoginTime { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}