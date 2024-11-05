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

            if (friend is null)
            {
                return false;
            }
            
            context.Friend.Remove(entity: friend);
            context.SaveChanges();

            return true;

        }

        public bool Accept(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }

            var friend = Find(userId: userId, friendId: friendId);

            if (friend is null)
            {
                return false;
            }
            
            friend.Status = 1;
            context.SaveChanges();

            return true;

        }

        public bool Decline(string userId, string friendId)
        {
            if (AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }

            var friend = Find(userId: userId, friendId: friendId);

            if (friend is null)
            {
                return false;
            }
            
            friend.Status = 2;
            context.SaveChanges();

            return true;
        }

        public bool Blocked(string userId, string friendId)
        {
            if (!AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }
            
            var friend = Find(userId: userId, friendId: friendId);
            if (friend is null)
            {
                return false;
            }
            
            friend.Status = 3;
            context.SaveChanges();

            return true;
        }

        public bool UnBlocked(string userId, string friendId)
        {
            if (!AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }
            
            var friend = Find(userId: userId, friendId: friendId);
            if (friend is null)
            {
                return false;
            }
                
            friend.Status = 1;
            context.SaveChanges();

            return true;
        }

        public bool Remove(string userId, string friendId)
        {
            if (!AreFriends(userId: userId, friendId: friendId))
            {
                return false;
            }
            
            var friend = Find(userId: userId, friendId: friendId);
            if (friend is null)
            {
                return false;
            }
                
            context.Friend.Remove(entity: friend);
            context.SaveChanges();
            return true;
        }

        public async Task<List<FriendViewModel>> GetFriends(string userId)
        {
            var friends = await context.Friend
                .Where(predicate: f => (f.UserId == userId || f.FriendId == userId)
                                       && f.Status == 1)
                .ToListAsync();

            var result = new List<FriendViewModel>();
            foreach (var friend in friends)
            {
                result.Add(item: await NewFriendViewModel(friend: friend));
            }
            return result;
        }

        public async Task<List<FriendViewModel>> GetFriendRequests(string userId)
        {
            var friends = await context.Friend
                .Where(predicate: f => (f.UserId == userId || f.FriendId == userId) && f.Status == 0)
                .ToListAsync();

            var result = new List<FriendViewModel>();
            foreach (var friend in friends)
            {
                result.Add(item: await NewFriendViewModel(friend: friend));
            }
            return result;
        }

        public async Task<List<FriendViewModel>> GetBlockedFriends(string userId)
        {
            var friends = await context.Friend
                .Where(predicate: f => (f.UserId == userId || f.FriendId == userId) && f.Status == 3)
                .ToListAsync();

            var result = new List<FriendViewModel>();
            foreach (var friend in friends)
            {
                result.Add(item: await NewFriendViewModel(friend: friend));
            }
            return result;
        }

        public bool AreFriends(string userId, string friendId)
        {
            var friend = Find(userId: userId, friendId: friendId);

            return friend is not null && friend.Status != 0;
        }

        public Friend? Find(string userId, string friendId)
        {
            return context.Friend
                    .FirstOrDefault(predicate: f =>
                        (f.UserId == userId && f.FriendId == friendId) ||
                        (f.UserId == friendId && f.FriendId == userId));
        }

        private async Task<FriendViewModel> NewFriendViewModel(Friend friend)
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
