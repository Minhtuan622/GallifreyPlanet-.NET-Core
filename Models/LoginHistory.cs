namespace GallifreyPlanet.Models
{
    public class LoginHistory
    {
        public Int64 Id { get; set; }
        public string? UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
