using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models;

[Table(name: "Conversations")]
public class Conversation
{
    public int Id { get; set; }
    public string? Members { get; set; }
    public bool IsGroup { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? GroupName { get; set; }
    public string? GroupDetail { get; set; }
    public string? GroupAvatar { get; set; }
    public string? CreatedBy { get; set; }
}