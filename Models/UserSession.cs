namespace GallifreyPlanet.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public string? DeviceName { get; set; }
        public string? Location { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
}
