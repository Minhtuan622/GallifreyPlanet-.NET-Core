using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Friend;
using Microsoft.EntityFrameworkCore;

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

        public bool Send(string UserId, string FriendId)
        {
            if (AreFriends(UserId, FriendId))
            {
                return false;
            }

            Friend friend = new Friend
            {
                UserId = UserId,
                FriendId = FriendId,
                Status = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.Add(friend);
            _context.SaveChanges();

            return true;
        }

        public bool Cancel(string UserId, string FriendId)
        {
            if (AreFriends(UserId, FriendId))
            {
                return false;
            }

            Friend? friend = Find(UserId, FriendId);

            if (friend is not null)
            {
                _context.Friend.Remove(friend);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Accept(string UserId, string FriendId)
        {
            if (AreFriends(UserId, FriendId))
            {
                return false;
            }

            Friend? friend = Find(UserId, FriendId);

            if (friend is not null)
            {
                friend.Status = 1;
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Decline(string UserId, string FriendId)
        {
            if (AreFriends(UserId, FriendId))
            {
                return false;
            }

            Friend? friend = Find(UserId, FriendId);

            if (friend is not null)
            {
                friend.Status = 2;
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Blocked(string UserId, string FriendId)
        {
            if (AreFriends(UserId, FriendId))
            {
                Friend? friend = Find(UserId, FriendId);

                if (friend is not null)
                {
                    friend.Status = 3;
                    _context.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public bool Remove(string UserId, string FriendId)
        {
            if (AreFriends(UserId, FriendId))
            {
                Friend? friend = Find(UserId, FriendId);

                if (friend is not null)
                {
                    _context.Friend.Remove(friend);
                    _context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public async Task<List<FriendViewModel>> GetFriends(string UserId)
        {
            List<Friend>? friends = await _context.Friend
                .Where(f => (f.UserId == UserId || f.FriendId == UserId)
                        && f.Status == 1)
                .ToListAsync();

            List<FriendViewModel>? result = new List<FriendViewModel>();
            foreach (Friend friend in friends)
            {
                result.Add(await NewFriendViewModel(friend));
            }
            return result;
        }

        public async Task<List<FriendViewModel>> GetFriendRequests(string UserId)
        {
            List<Friend>? friends = await _context.Friend
                .Where(f => (f.UserId == UserId || f.FriendId == UserId)
                        && f.Status == 0)
                .ToListAsync();

            List<FriendViewModel>? result = new List<FriendViewModel>();
            foreach (Friend friend in friends)
            {
                result.Add(await NewFriendViewModel(friend));
            }
            return result;
        }

        public bool AreFriends(string UserId, string FriendId)
        {
            Friend? friend = Find(UserId, FriendId);

            if (friend is not null && friend.Status != 0)
            {
                return true;
            }
            return false;
        }

        public Friend? Find(string UserId, string FriendId)
        {
            return _context.Friend
                    .FirstOrDefault(f =>
                        (f.UserId == UserId && f.FriendId == FriendId) ||
                        (f.UserId == FriendId && f.FriendId == UserId));
        }

        public async Task<FriendViewModel> NewFriendViewModel(Friend friend)
        {
            return new FriendViewModel
            {
                User = await _userService.GetUserAsyncById(friend.UserId!),
                Friend = await _userService.GetUserAsyncById(friend.FriendId!),
                Status = friend.Status,
                CreatedAt = friend.CreatedAt,
                UpdatedAt = friend.UpdatedAt,
            };
        }
    }
}
