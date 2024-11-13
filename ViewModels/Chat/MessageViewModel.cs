using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.Chat;

public class MessageViewModel
{
    public int Id { get; set; }
    public ConversationViewModel? Conversation { get; set; }
    public User? Sender { get; set; }
    public string? Content { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}