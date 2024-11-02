using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels;
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

        public bool Send(string userId, string friendId)
        {
            if (AreFriends(userId, friendId))
            {
                return false;
            }

            Friend friend = new Friend
            {
                UserId = userId,
                FriendId = friendId,
                Status = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            _context.Add(friend);
            _context.SaveChanges();

            return true;
        }

        public bool Cancel(string userId, string friendId)
        {
            if (AreFriends(userId, friendId))
            {
                return false;
            }

            Friend? friend = Find(userId, friendId);

            if (friend is not null)
            {
                _context.Friend.Remove(friend);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Accept(string userId, string friendId)
        {
            if (AreFriends(userId, friendId))
            {
                return false;
            }

            Friend? friend = Find(userId, friendId);

            if (friend is not null)
            {
                friend.Status = 1;
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Decline(string userId, string friendId)
        {
            if (AreFriends(userId, friendId))
            {
                return false;
            }

            Friend? friend = Find(userId, friendId);

            if (friend is not null)
            {
                friend.Status = 2;
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Blocked(string userId, string friendId)
        {
            if (AreFriends(userId, friendId))
            {
                Friend? friend = Find(userId, friendId);

                if (friend is not null)
                {
                    friend.Status = 3;
                    _context.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public bool UnBlocked(string userId, string friendId)
        {
            if (AreFriends(userId, friendId))
            {
                Friend? friend = Find(userId, friendId);

                if (friend is not null)
                {
                    friend.Status = 1;
                    _context.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public bool Remove(string userId, string friendId)
        {
            if (AreFriends(userId, friendId))
            {
                Friend? friend = Find(userId, friendId);

                if (friend is not null)
                {
                    _context.Friend.Remove(friend);
                    _context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public async Task<List<FriendViewModel>> GetFriends(string userId)
        {
            List<Friend> friends = await _context.Friend
                .Where(f => (f.UserId == userId || f.FriendId == userId)
                        && f.Status == 1)
                .ToListAsync();

            List<FriendViewModel> result = new List<FriendViewModel>();
            foreach (Friend friend in friends)
            {
                result.Add(await NewFriendViewModel(friend));
            }
            return result;
        }

        public async Task<List<FriendViewModel>> GetFriendRequests(string userId)
        {
            List<Friend> friends = await _context.Friend
                .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == 0)
                .ToListAsync();

            List<FriendViewModel> result = new List<FriendViewModel>();
            foreach (Friend friend in friends)
            {
                result.Add(await NewFriendViewModel(friend));
            }
            return result;
        }

        public async Task<List<FriendViewModel>> GetBlockedFriends(string userId)
        {
            List<Friend> friends = await _context.Friend
                .Where(f => (f.UserId == userId || f.FriendId == userId) && f.Status == 3)
                .ToListAsync();

            List<FriendViewModel> result = new List<FriendViewModel>();
            foreach (Friend friend in friends)
            {
                result.Add(await NewFriendViewModel(friend));
            }
            return result;
        }

        public bool AreFriends(string userId, string friendId)
        {
            Friend? friend = Find(userId, friendId);

            if (friend is not null && friend.Status != 0)
            {
                return true;
            }
            return false;
        }

        public Friend? Find(string userId, string friendId)
        {
            return _context.Friend
                    .FirstOrDefault(f =>
                        (f.UserId == userId && f.FriendId == friendId) ||
                        (f.UserId == friendId && f.FriendId == userId));
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
