using System.ComponentModel.DataAnnotations;
using GallifreyPlanet.Models;

namespace GallifreyPlanet.ViewModels.Chat;

public class ConversationViewModel
{
    public int Id { get; set; }
    public List<User?>? Members { get; set; }
    public bool IsGroup { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? LatestMessage { get; set; }

    [Display(Name = "Tên nhóm", Prompt = "Nhập tên nhóm...")]
    [Required(ErrorMessage = "Vui lòng nhập tên nhóm")]
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Tên nhóm phải từ 3-50 ký tự")]
    public string? GroupName { get; set; }

    [Display(Name = "Mô tả nhóm", Prompt = "Nhập mô tả nhóm...")]
    [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Mô tả nhóm phải từ 5-100 ký tự")]
    public string? GroupDetail { get; set; }

    [Display(Name = "Ảnh nhóm")] public IFormFile? GroupAvatar { get; set; }
    public string? CurrentGroupAvatar { get; set; }
    public User? CreatedBy { get; set; }
}