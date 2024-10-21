using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.Friend
{
    public class FriendViewModel
    {
        public User? User { get; set; }
        public User? Friend { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
