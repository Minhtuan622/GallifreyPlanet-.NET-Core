using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.Chat;

public class ChatManagerViewModel
{
    public User? User { get; set; }
    public List<ConversationViewModel>? Conversations { get; set; }
    public ConversationViewModel? SelectedConversation { get; set; }
    public ConversationViewModel? NewConversation { get; set; }
    public List<MessageViewModel>? Messages { get; set; }
    public List<string>? SelectedMemberIds { get; set; }
}