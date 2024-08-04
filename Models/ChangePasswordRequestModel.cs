using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models
{
    public class ChangePasswordRequestModel
    {
        [Required, Length(26, 26)]
        public string UserId { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Password must be at least 3 characters long"), MaxLength(50, ErrorMessage = "Password must be at most 50 characters long")]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
