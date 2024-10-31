using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels
{
    public class MessageViewModel
    {
        public ConversationViewModel? Conversation { get; set; }
        public User? Sender { get; set; }
        public User? Receiver { get; set; }
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
