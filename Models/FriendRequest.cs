using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models
{
    [Table(name: "FriendRequests")]
    public class FriendRequest
    {
        public int Id { get; set; }
        public string? RequesterId { get; set; }
        public string? ReceiverId { get; set; }
        public int? Status { get; set; } // 0: pending, 1: accept, 2: decline
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
