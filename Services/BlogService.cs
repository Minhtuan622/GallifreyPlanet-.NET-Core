using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Blog;

namespace GallifreyPlanet.Services
{
    public class BlogService
    {
        private readonly GallifreyPlanetContext _context;

        public BlogService(GallifreyPlanetContext context)
        {
            _context = context;
        }

        public Task<List<BlogViewModel>> GetBlogsByUserId(string userId, int? count = null)
        {
            List<Blog>? blogs = _context.Blog
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Id)
                .ToList();

            if (count is not null)
            {
                blogs = blogs.Take(count.Value).ToList();
            }

            return Task.FromResult(blogs.Select(NewBlogViewModel).ToList());
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
