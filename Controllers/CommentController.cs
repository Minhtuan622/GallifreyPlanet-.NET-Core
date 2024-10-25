using GallifreyPlanet.Data;
using GallifreyPlanet.Models;
using GallifreyPlanet.Services;
using Microsoft.AspNetCore.Mvc;

namespace GallifreyPlanet.Controllers
{
    public class CommentController : Controller
    {
        private readonly GallifreyPlanetContext _context;
        private readonly UserService _userService;
        private readonly CommentService _commentService;

        public CommentController(
            GallifreyPlanetContext context,
            UserService userService,
            CommentService commentService
        )
        {
            _context = context;
            _userService = userService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<JsonResult> Get(int commentableId)
        {
            try
            {
                User? user = await _userService.GetCurrentUserAsync();
                if (user is null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập để sử dụng tính năng này",
                    });
                }

                return Json(new
                {
                    success = true,
                    data = await _commentService.Get(CommentableType.blog, commentableId),
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.ToString(),
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Add(int commentableId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bình luận không được để trống",
                    });
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Vui lòng đăng nhập để sử dụng tính năng này",
                    });
                }

                Comment? comment = new Comment
                {
                    UserId = user.Id,
                    CommentableId = commentableId,
                    CommentableType = CommentableType.blog,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await _context.Comment.AddAsync(comment);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Bình luận thành công",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.ToString(),
                });
            }
        }

        [HttpDelete]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                Comment? comment = _commentService.GetById(id);
                if (comment == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bình luận không được để trống",
                    });
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null || user.Id != comment.UserId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập để sử dụng tính năng này",
                    });
                }


                if (_commentService.DeleteCommentChildren(comment))
                {
                    return Json(new
                    {
                        success = true,
                        message = "Xóa bình luận thành công",
                    });
                }

                return Json(new
                {
                    success = false,
                    message = "Xóa bình luận không thành công",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.ToString(),
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AddReply(int commentId, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bình luận không được để trống",
                    });
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập để sử dụng tính năng này",
                    });
                }

                Comment? comment = _commentService.GetById(commentId);
                if (comment == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bình luận bạn vừa phản hồi không còn tồn tại",
                    });
                }

                Comment? reply = new Comment
                {
                    ParentId = commentId,
                    CommentableType = CommentableType.blog,
                    CommentableId = comment.CommentableId,
                    UserId = user.Id,
                    Content = content.Trim(),
                    CreatedAt = DateTime.Now
                };

                await _context.Comment.AddAsync(reply);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.ToString(),
                });
            }
        }

        [HttpDelete]
        public async Task<JsonResult> DeleteReply(int id)
        {
            try
            {
                Comment? reply = _commentService.GetById(id);
                if (reply == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bình luận không tồn tại",
                    });
                }

                User? user = await _userService.GetCurrentUserAsync();
                if (user == null || user.Id != reply.UserId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Vui lòng đăng nhập để sử dụng tính năng này",
                    });
                }

                _context.Comment.Remove(reply);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Xóa bình luận thành công",
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.ToString(),
                });
            }
        }
    }
}
