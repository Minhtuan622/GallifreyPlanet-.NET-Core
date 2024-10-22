using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using GallifreyPlanet.ViewModels.Blog;
using Microsoft.AspNetCore.Authorization;
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

        public BlogsController(
            GallifreyPlanetContext context,
            UserService userService,
            BlogService blogService,
            FileService fileService
        )
        {
            _context = context;
            _userService = userService;
            _blogService = blogService;
            _fileService = fileService;
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
        public async Task<IActionResult> Details(int? id)
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

            BlogManagerViewModel? viewModel = new BlogManagerViewModel
            {
                User = await _userService.GetCurrentUserAsync(),
                BlogViewModel = _blogService.NewBlogViewModel(blog),
                Comment = null,
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
                    if (!BlogExists(blog.Id))
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

        private bool BlogExists(long id)
        {
            return _context.Blog.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(int blogId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return BadRequest(error: "Nội dung bình luận không được để trống");
                }

                // Kiểm tra blog có tồn tại
                Blog? blog = await _context.Blog.FindAsync(blogId);
                if (blog == null)
                {
                    return NotFound(value: "Không tìm thấy bài viết");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                Comment? comment = new Comment
                {
                    BlogId = blogId,
                    UserId = user.Id,
                    Content = content.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Comment.AddAsync(comment);
                await _context.SaveChangesAsync();

                // Nếu là Ajax request thì trả về partial view
                if (Request.Headers[key: "X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(viewName: "_CommentPartial", comment);
                }

                // Nếu không phải Ajax thì redirect về trang blog
                return RedirectToAction(actionName: "Details", new { id = blogId });
            }
            catch (Exception ex)
            {
                return StatusCode(statusCode: 500, value: "Đã xảy ra lỗi khi thêm bình luận");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                Comment? comment = await _context.Comment
                    .Include(c => c.Replies)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (comment == null)
                {
                    return NotFound("Không tìm thấy bình luận");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null || user.Id != comment.UserId)
                {
                    return Unauthorized("Bạn không có quyền xóa bình luận này");
                }

                // Xóa các reply trước
                if (comment.Replies?.Any() == true)
                {
                    _context.Reply.RemoveRange(comment.Replies);
                }

                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi xóa bình luận");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReply(int commentId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return BadRequest("Nội dung phản hồi không được để trống");
                }

                // Kiểm tra comment có tồn tại
                Comment? comment = await _context.Comment.FindAsync(commentId);
                if (comment == null)
                {
                    return NotFound("Không tìm thấy bình luận");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null)
                {
                    return Unauthorized();
                }

                Reply? reply = new Reply
                {
                    CommentId = commentId,
                    UserId = user.Id,
                    Content = content.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Reply.AddAsync(reply);
                await _context.SaveChangesAsync();

                // Nếu là Ajax request thì trả về partial view
                if (Request.Headers[key: "X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView(viewName: "_ReplyPartial", reply);
                }

                // Nếu không phải Ajax thì redirect về trang blog
                return RedirectToAction(actionName: "Details", new { id = comment.BlogId });
            }
            catch (Exception ex)
            {
                return StatusCode(statusCode: 500, value: "Đã xảy ra lỗi khi thêm phản hồi");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteReply(int id)
        {
            try
            {
                Reply? reply = await _context.Reply.FindAsync(id);
                if (reply == null)
                {
                    return NotFound("Không tìm thấy phản hồi");
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null || user.Id != reply.UserId)
                {
                    return Unauthorized("Bạn không có quyền xóa phản hồi này");
                }

                _context.Reply.Remove(reply);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(statusCode: 500, value: "Đã xảy ra lỗi khi xóa phản hồi");
            }
        }
    }
}
