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
            var user = await userService.GetCurrentUserAsync();
            if (user is null)
            {
                return NotFound();
            }

            var blog = await blogService.GetBlogsByUserId(userId: user.Id);
            return View(model: blog);
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int id, string userId)
        {
            var user = await userService.GetUserAsyncById(userId: userId);

            if (user is null || string.IsNullOrEmpty(value: user.Id))
            {
                return NotFound();
            }

            var blog = await context.Blog.FirstOrDefaultAsync(predicate: m => m.Id == id);
            if (blog is null)
            {
                return NotFound();
            }

            var viewModel = new BlogManagerViewModel
            {
                User = user,
                BlogViewModel = blogService.NewBlogViewModel(blog: blog),
                Comments = await commentService.Get(commentableType: CommentableType.Blog, commentableId: id)
            };

            return View(model: viewModel);
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
                var user = await userService.GetCurrentUserAsync();
                var blog = new Blog
                {
                    UserId = user!.Id,
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Content = viewModel.Content,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                if (viewModel.ThumbnailFile is { Length: > 0 })
                {
                    var file = await fileService.UploadFileAsync(
                        file: viewModel.ThumbnailFile,
                        folder: "/blogs",
                        currentFilePath: viewModel.CurrentThumbnailPath!
                    );
                    blog.ThumbnailPath = file;
                }

                context.Add(entity: blog);
                await context.SaveChangesAsync();
                TempData[key: "StatusMessage"] = "Tạo thành công";
                return RedirectToAction(actionName: nameof(Index));
            }
            return View(model: viewModel);
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var blog = await context.Blog.FindAsync(keyValues: id);
            if (blog is null)
            {
                return NotFound();
            }

            var viewModel = new BlogViewModel
            {
                Id = (int)id,
                Title = blog.Title,
                Description = blog.Description,
                Content = blog.Content,
                CurrentThumbnailPath = blog.ThumbnailPath
            };

            return View(model: viewModel);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogViewModel viewModel)
        {
            var blog = await context.Blog.FindAsync(keyValues: id);
            if (id != blog!.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model: viewModel);
            }
            
            try
            {
                blog.Title = viewModel.Title;
                blog.Description = viewModel.Description;
                blog.Content = viewModel.Content;
                blog.UpdatedAt = DateTime.Now;

                if (viewModel.ThumbnailFile is { Length: > 0 })
                {
                    var file = await fileService.UploadFileAsync(file: viewModel.ThumbnailFile, folder: "/blogs", currentFilePath: blog.ThumbnailPath!);
                    blog.ThumbnailPath = file;
                }

                context.Update(entity: blog);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!blogService.BlogExists(id: blog.Id))
                {
                    return NotFound();
                }

                throw;
            }

            TempData[key: "StatusMessage"] = "Cập nhật thành công";
            return RedirectToAction(actionName: nameof(Index));
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var blog = await context.Blog.FirstOrDefaultAsync(predicate: m => m.Id == id);
            if (blog is null)
            {
                return NotFound();
            }

            var blogViewModel = new BlogManagerViewModel
            {
                User = await userService.GetCurrentUserAsync(),
                BlogViewModel = blogService.NewBlogViewModel(blog: blog),
            };

            return View(model: blogViewModel);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName(name: "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await context.Blog.FindAsync(keyValues: id);
            if (blog is not null)
            {
                context.Blog.Remove(entity: blog);
            }

            await context.SaveChangesAsync();
            TempData[key: "StatusMessage"] = "Xóa thành công";
            return RedirectToAction(actionName: nameof(Index));
        }
    }
}
