using System.ComponentModel.DataAnnotations.Schema;

namespace GallifreyPlanet.Models;

[Table(name: "Notifications")]
public class Notification
{
    public int Id { get; set; }
    public string? SenderId { get; set; }
    public int Type { get; set; }
    public string? Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}