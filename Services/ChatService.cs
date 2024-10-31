using GallifreyPlanet.Data;

namespace GallifreyPlanet.Services
{
    public class ChatService
    {
        private readonly GallifreyPlanetContext _context;

        public ChatService(GallifreyPlanetContext context)
        {
            _context = context;
        }

        public bool CreateConversation()
        {
            return true;
        }
    }
}
