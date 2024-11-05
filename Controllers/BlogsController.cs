using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Controllers
{
    public class BlogsController(
        GallifreyPlanetContext context,
        UserService userService,
        BlogService blogService,
        FileService fileService,
        CommentService commentService)
        : Controller
    {
        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            User? user = await userService.GetCurrentUserAsync();
            if (user is null)
            {
                return NotFound();
            }

            List<BlogViewModel> blog = await blogService.GetBlogsByUserId(user.Id);
            return View(blog);
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int id, string userId)
        {
            User? user = await userService.GetUserAsyncById(userId);

            if (user is null || string.IsNullOrEmpty(user.Id))
            {
                return NotFound();
            }

            Blog? blog = await context.Blog.FirstOrDefaultAsync(m => m.Id == id);
            if (blog is null)
            {
                return NotFound();
            }

            BlogManagerViewModel viewModel = new BlogManagerViewModel
            {
                User = user,
                BlogViewModel = blogService.NewBlogViewModel(blog),
                Comments = await commentService.Get(CommentableType.Blog, id)
            };

            return View(viewModel);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                User? user = await userService.GetCurrentUserAsync();
                Blog blog = new Blog
                {
                    UserId = user!.Id,
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Content = viewModel.Content,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                if (viewModel.ThumbnailFile != null && viewModel.ThumbnailFile.Length > 0)
                {
                    string file = await fileService.UploadFileAsync(
                        viewModel.ThumbnailFile,
                        folder: "/blogs",
                        viewModel.CurrentThumbnailPath!
                    );
                    blog.ThumbnailPath = file;
                }

                context.Add(blog);
                await context.SaveChangesAsync();
                TempData[key: "StatusMessage"] = "Tạo thành công";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Blog? blog = await context.Blog.FindAsync(id);
            if (blog is null)
            {
                return NotFound();
            }

            BlogViewModel viewModel = new BlogViewModel
            {
                Id = id ?? 0,
                Title = blog.Title,
                Description = blog.Description,
                Content = blog.Content,
                CurrentThumbnailPath = blog.ThumbnailPath
            };

            return View(viewModel);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogViewModel viewModel)
        {
            Blog? blog = await context.Blog.FindAsync(id);
            if (id != blog!.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blog.Title = viewModel.Title;
                    blog.Description = viewModel.Description;
                    blog.Content = viewModel.Content;
                    blog.UpdatedAt = DateTime.Now;

                    if (viewModel.ThumbnailFile != null && viewModel.ThumbnailFile.Length > 0)
                    {
                        string file = await fileService.UploadFileAsync(viewModel.ThumbnailFile, folder: "/blogs", blog.ThumbnailPath!);
                        blog.ThumbnailPath = file;
                    }

                    context.Update(blog);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!blogService.BlogExists(blog.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                TempData[key: "StatusMessage"] = "Cập nhật thành công";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            Blog? blog = await context.Blog.FirstOrDefaultAsync(m => m.Id == id);
            if (blog is null)
            {
                return NotFound();
            }

            BlogManagerViewModel blogViewModel = new BlogManagerViewModel
            {
                User = await userService.GetCurrentUserAsync(),
                BlogViewModel = blogService.NewBlogViewModel(blog),
            };

            return View(blogViewModel);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName(name: "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Blog? blog = await context.Blog.FindAsync(id);
            if (blog is not null)
            {
                context.Blog.Remove(blog);
            }

            await context.SaveChangesAsync();
            TempData[key: "StatusMessage"] = "Xóa thành công";
            return RedirectToAction(nameof(Index));
        }
    }
}
