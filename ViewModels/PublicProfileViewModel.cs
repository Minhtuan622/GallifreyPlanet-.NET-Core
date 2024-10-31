using GallifreyPlanet.ViewModels.Blog;
using GallifreyPlanet.ViewModels.Friend;

namespace GallifreyPlanet.ViewModels
{
    public class PublicProfileViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Website { get; set; }
        public string? Github { get; set; }
        public string? Twitter { get; set; }
        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public bool IsFriend { get; set; }
        public bool IsSendRequest { get; set; }
        public bool AllowChat { get; set; }
        public bool AllowAddFriend { get; set; }
        public RecentActivities RecentActivities { get; set; } = new RecentActivities();
        public List<BlogViewModel> RecentBlogs { get; set; } = new List<BlogViewModel>();
        public List<FriendViewModel>? Friends { get; set; }
    }

    public class RecentActivities
    {
        public int CommentPercentage { get; set; }
        public int LikePercentage { get; set; }
        public int SharePercentage { get; set; }
        public int RatingPercentage { get; set; }
        public int FollowPercentage { get; set; }
    }
}
