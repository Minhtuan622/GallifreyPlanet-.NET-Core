using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.ViewModels.Blog;

namespace GallifreyPlanet.Services;

public class BlogService(GallifreyPlanetContext context)
{
    public BlogViewModel? GetById(int id)
    {
        return context.Blog
            .Select(selector: blog => NewBlogViewModel(blog))
            .FirstOrDefault(b => b.Id == id);
    }

    public Task<List<BlogViewModel>> GetBlogsByUserId(string userId, int? count = null)
    {
        var blogs = context.Blog
            .Where(predicate: b => b.UserId == userId)
            .OrderByDescending(keySelector: b => b.Id)
            .ToList();

        if (count is not null)
        {
            blogs = blogs.Take(count: count.Value).ToList();
        }

        return Task.FromResult(result: blogs.Select(selector: NewBlogViewModel).ToList());
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

    public bool BlogExists(int id)
    {
        return context.Blog.Any(predicate: e => e.Id == id);
    }
}