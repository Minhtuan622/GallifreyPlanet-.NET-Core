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

        public async Task<List<BlogViewModel>> GetBlogsByUserId(string userId, int? count = null)
        {
            List<Blog>? blogs = await _gallifreyPlanetContext.Blog
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            if (count is not null)
            {
                blogs = blogs.Take(count.Value).ToList();
            }

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
                CurrentThumbnailPath = blog.ThumbnailPath,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt,
            };
        }
    }
}
