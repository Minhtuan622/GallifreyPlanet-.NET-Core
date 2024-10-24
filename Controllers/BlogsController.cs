using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GallifreyPlanet.Controllers
{
    public class BlogsController : Controller
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;
        private readonly BlogService _blogService;
        private readonly FileService _fileService;
        private readonly CommentService _commentService;

        public BlogsController(
            GallifreyPlanetContext context,
            UserService userService,
            BlogService blogService,
            FileService fileService,
            CommentService commentService
        )
        {
            _context = context;
            _userService = userService;
            _blogService = blogService;
            _fileService = fileService;
            _commentService = commentService;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            User? user = await _userService.GetCurrentUserAsync();
            if (user is null)
            {
                return NotFound();
            }

            List<BlogViewModel>? blog = await _blogService.GetBlogsByUserId(user.Id);
            return View(blog);
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            User? user = await _userService.GetCurrentUserAsync();

            if (user is null || string.IsNullOrEmpty(user.Id))
            {
                return NotFound();
            }

            Blog? blog = await _context.Blog.FirstOrDefaultAsync(m => m.Id == id);
            if (blog is null)
            {
                return NotFound();
            }

            BlogManagerViewModel viewModel = new BlogManagerViewModel
            {
                User = user,
                BlogViewModel = _blogService.NewBlogViewModel(blog),
                Comments = await _commentService.GetComments(CommentableType.blog, id)
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
                User? user = await _userService.GetCurrentUserAsync();
                Blog? blog = new Blog
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
                    string? file = await _fileService.UploadFileAsync(viewModel.ThumbnailFile, uploadFolder: "/blogs");
                    blog.ThumbnailPath = file;
                }

                _context.Add(blog);
                await _context.SaveChangesAsync();
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

            Blog? blog = await _context.Blog.FindAsync(id);
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
            Blog? blog = await _context.Blog.FindAsync(id);
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
                        string? file = await _fileService.UploadFileAsync(viewModel.ThumbnailFile, uploadFolder: "/blogs");
                        blog.ThumbnailPath = file;
                    }


                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_blogService.BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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

            Blog? blog = await _context.Blog.FirstOrDefaultAsync(m => m.Id == id);
            if (blog is null)
            {
                return NotFound();
            }

            BlogManagerViewModel? blogViewModel = new BlogManagerViewModel
            {
                User = await _userService.GetCurrentUserAsync(),
                BlogViewModel = _blogService.NewBlogViewModel(blog),
            };

            return View(blogViewModel);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName(name: "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Blog? blog = await _context.Blog.FindAsync(id);
            if (blog is not null)
            {
                _context.Blog.Remove(blog);
            }

            await _context.SaveChangesAsync();
            TempData[key: "StatusMessage"] = "Xóa thành công";
            return RedirectToAction(nameof(Index));
        }
    }
}
