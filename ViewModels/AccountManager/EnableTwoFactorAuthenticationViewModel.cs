using System.ComponentModel.DataAnnotations;

namespace GallifreyPlanet.ViewModels.AccountManager
{
    public class EnableTwoFactorAuthenticationViewModel
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã xác thực.")]
        [StringLength(7, MinimumLength = 6, ErrorMessage = "Mã xác thực phải có 6 ký tự.")]
        [Display(Name = "Mã xác thực")]
        public string VerificationCode { get; set; }
    }
}