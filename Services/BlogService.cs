using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Blog;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Services
{
    public class BlogService
    {
        private readonly GallifreyPlanetContext _gallifreyPlanetContext;

        public BlogService(GallifreyPlanetContext gallifreyPlanetContext)
        {
            _gallifreyPlanetContext = gallifreyPlanetContext;
        }

        public async Task<List<BlogViewModel>> GetRecentBlogsByUser(string userId, int count)
        {
            List<Blog>? blogs = await _gallifreyPlanetContext.Blog
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Id)
                .Take(count)
                .ToListAsync();

            return blogs.Select(NewBlogViewModel).ToList();
        }

        public BlogViewModel NewBlogViewModel(Blog blog)
        {
            return new BlogViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Description = blog.Description,
                UserId = blog.UserId,
                CurrentThumbnailPath = blog.ThumbnailPath
            };
        }
    }
}
