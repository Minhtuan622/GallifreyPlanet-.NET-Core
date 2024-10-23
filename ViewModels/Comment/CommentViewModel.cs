using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Blog;

namespace GallifreyPlanet.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public int CommentableId { get; set; }
        public CommentableType CommentableType { get; set; }
        public BlogViewModel? Blog { get; set; }
        public string? Content { get; set; }
        public List<ReplyViewModel>? Replies { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
