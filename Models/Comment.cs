namespace GallifreyPlanet.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int BlogId { get; set; }
        public string? Content { get; set; }
        public ICollection<Reply>? Replies { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
