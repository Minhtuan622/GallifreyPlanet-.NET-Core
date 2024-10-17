using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models
{
    [Table(name: "friends")]
    public class Friend
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? FriendId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
