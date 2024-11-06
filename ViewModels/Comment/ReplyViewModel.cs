using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.Comment;

public class ReplyViewModel
{
    public int Id { get; set; }
    public User? User { get; set; }
    public CommentViewModel? Comment { get; set; }
    public string? Content { get; set; }
    public DateTime? CreatedAt { get; set; }
}