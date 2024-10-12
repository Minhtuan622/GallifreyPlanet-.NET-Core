using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Blog;

namespace GallifreyPlanet.Services
{
    public class BlogService
    {
        private readonly GallifreyPlanetContext _gallifreyPlanetContext;

        public BlogService(GallifreyPlanetContext gallifreyPlanetContext)
        {
            _gallifreyPlanetContext = gallifreyPlanetContext;
        }

        public BlogViewModel NewBlogViewModel(Blog blog)
        {
            return new BlogViewModel
            {
                Title = blog.Title,
                Content = blog.Content,
                Description = blog.Description,
                UserId = blog.UserId,
                CurrentThumbnailPath = blog.ThumbnailPath
            };
        }
    }
}
