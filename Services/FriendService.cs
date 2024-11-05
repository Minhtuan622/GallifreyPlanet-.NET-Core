using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Friend;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services
{
    public class FriendService(
        GallifreyPlanetContext context,
        UserService userService)
    {
        public bool Send(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }

            var friend = new Friend
            {
                UserId = userId,
                FriendId = friendId,
                Status = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            context.Add(entity: friend);
            context.SaveChanges();

            return true;
        }

        public bool Cancel(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }

            var friend = Find(userId: userId, friendId: friendId);

            if (friend is not null)
            {
                context.Friend.Remove(entity: friend);
                context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Accept(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }

            var friend = Find(userId: userId, friendId: friendId);

            if (friend is not null)
            {
                friend.Status = 1;
                context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Decline(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }

            var friend = Find(userId: userId, friendId: friendId);

            if (friend is not null)
            {
                friend.Status = 2;
                context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Blocked(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                var friend = Find(userId: userId, friendId: friendId);

                if (friend is not null)
                {
                    friend.Status = 3;
                    context.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public bool UnBlocked(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                var friend = Find(userId: userId, friendId: friendId);

                if (friend is not null)
                {
                    friend.Status = 1;
                    context.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public bool Remove(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                var friend = Find(userId: userId, friendId: friendId);

                if (friend is not null)
                {
                    context.Friend.Remove(entity: friend);
                    context.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public async Task<List<FriendViewModel>> GetFriends(string userId)
        {
            List<Friend> friends = await context.Friend
                .Where(predicate: f => (f.UserId == userId || f.FriendId == userId)
                                       && f.Status == 1)
                .ToListAsync();

            List<FriendViewModel> result = new List<FriendViewModel>();
            foreach (var friend in friends)
            {
                result.Add(item: await NewFriendViewModel(friend: friend));
            }
            return result;
        }

        public async Task<List<FriendViewModel>> GetFriendRequests(string userId)
        {
            List<Friend> friends = await context.Friend
                .Where(predicate: f => (f.UserId == userId || f.FriendId == userId) && f.Status == 0)
                .ToListAsync();

            List<FriendViewModel> result = new List<FriendViewModel>();
            foreach (var friend in friends)
            {
                result.Add(item: await NewFriendViewModel(friend: friend));
            }
            return result;
        }

        public async Task<List<FriendViewModel>> GetBlockedFriends(string userId)
        {
            List<Friend> friends = await context.Friend
                .Where(predicate: f => (f.UserId == userId || f.FriendId == userId) && f.Status == 3)
                .ToListAsync();

            List<FriendViewModel> result = new List<FriendViewModel>();
            foreach (var friend in friends)
            {
                result.Add(item: await NewFriendViewModel(friend: friend));
            }
            return result;
        }

        public bool AreFriends(string userId, string friendId)
        {
            var friend = Find(userId: userId, friendId: friendId);

            if (friend is not null && friend.Status != 0)
            {
                return true;
            }
            return false;
        }

        public Friend? Find(string userId, string friendId)
        {
            return context.Friend
                    .FirstOrDefault(predicate: f =>
                        (f.UserId == userId && f.FriendId == friendId) ||
                        (f.UserId == friendId && f.FriendId == userId));
        }

        public async Task<FriendViewModel> NewFriendViewModel(Friend friend)
        {
            return new FriendViewModel
            {
                User = await userService.GetUserAsyncById(userId: friend.UserId!),
                Friend = await userService.GetUserAsyncById(userId: friend.FriendId!),
                Status = friend.Status,
                CreatedAt = friend.CreatedAt,
                UpdatedAt = friend.UpdatedAt,
            };
        }
    }
}
