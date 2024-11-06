using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models;

[Table(name: "Blogs")]
public class Blog
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Content { get; set; }
    public string? ThumbnailPath { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}