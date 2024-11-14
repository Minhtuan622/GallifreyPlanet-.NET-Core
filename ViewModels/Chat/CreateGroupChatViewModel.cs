using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.Chat;

public class CreateGroupChatViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên nhóm")]
    [StringLength(maximumLength: 100, MinimumLength = 3, ErrorMessage = "Tên nhóm phải từ 3-100 ký tự")]
    public string? GroupName { get; set; }

    public IFormFile? GroupAvatar { get; set; }

    public List<string>? SelectedMemberIds { get; set; }
}