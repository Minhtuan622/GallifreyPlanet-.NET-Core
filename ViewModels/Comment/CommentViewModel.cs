using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.Comment;

public class CommentViewModel
{
    public int Id { get; set; }
    public User? User { get; set; }
    public int CommentableId { get; set; }
    public CommentableType CommentableType { get; set; }
    public string? Content { get; set; }
    public int? ParentId { get; set; }
    public List<CommentViewModel>? Replies { get; set; }
    public DateTime? CreatedAt { get; set; }
}