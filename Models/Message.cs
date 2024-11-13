using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models;

[Table(name: "Messages")]
public class Message
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public string? SenderId { get; set; }
    public string? Content { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}