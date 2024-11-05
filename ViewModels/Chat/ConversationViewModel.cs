using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.Chat
{
    public class ConversationViewModel
    {
        public int Id { get; set; }
        public List<User?>? Members { get; set; }
        public string? GroupName { get; set; }
        public bool IsGroup { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LatestMessage { get; set; }
    }
}
