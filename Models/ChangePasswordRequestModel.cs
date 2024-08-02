using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models
{
    public class ChangePasswordRequestModel
    {
        [Required, Length(26, 26)]
        public int UserId { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
