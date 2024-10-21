using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models
{
    [Table(name: "Friends")]
    public class Friend
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? FriendId { get; set; }
        public int? Status { get; set; } // 0: pending, 1: accept, 2: decline, 3: blocked
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
