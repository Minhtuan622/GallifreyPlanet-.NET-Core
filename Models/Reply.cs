using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models
{
    [Table(name: "Replies")]
    public class Reply
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int ParentId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
