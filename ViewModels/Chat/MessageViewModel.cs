using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Chat;

namespace GallifreyPlanet.ViewModels.Chat
{
    public class MessageViewModel
    {
        public ConversationViewModel? Conversation { get; set; }
        public User? Sender { get; set; }
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
