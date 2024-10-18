using GallifreyPlanet.Data;
using GallifreyPlanet.Models;

namespace GallifreyPlanet.Services
{
    public class FriendService
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;

        public FriendService(
            GallifreyPlanetContext context,
            UserService userService
        )
        {
            _context = context;
            _userService = userService;
        }

        public async Task SendFriendRequest(string userId, string receiverId)
        {
            FriendRequest? newFriendRequest = new FriendRequest
            {
                RequesterId = userId,
                ReceiverId = receiverId,
                Status = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.FriendRequest.Add(newFriendRequest);
            await _context.SaveChangesAsync();
        }

        public FriendRequest? GetFriendRequestById(int friendRequestId)
        {
            return _context.FriendRequest.Find(friendRequestId);
        }

        public FriendRequest? GetFriendRequestByReceiverId(string ReceiverId, string UserId)
        {
            return _context.FriendRequest
                .Where(f => f.ReceiverId == ReceiverId && f.RequesterId == UserId)
                .FirstOrDefault();
        }

        public bool IsFriend(string friendId)
        {
            Friend? isFriend = _context.Friend
                .Where(r => r.FriendId == friendId)
                .FirstOrDefault();

            if (isFriend is not null)
            {
                return true;
            }
            return false;
        }
    }
}
