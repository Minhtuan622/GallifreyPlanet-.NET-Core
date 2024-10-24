using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models
{
    [Table(name: "Comments")]
    public class Comment
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int CommentableId { get; set; }
        public CommentableType CommentableType { get; set; }
        public string? Content { get; set; }
        public int? ParentId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public enum CommentableType
    {
        blog,
    }
}
