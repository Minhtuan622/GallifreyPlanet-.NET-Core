using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Comment;

namespace GallifreyPlanet.ViewModels.Blog
{
    public class BlogManagerViewModel
    {
        public User? User { get; set; }
        public BlogViewModel? BlogViewModel { get; set; }
        public List<CommentViewModel>? Comments { get; set; }
    }
}
