using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models
{
    public class ChangePasswordRequestModel
    {
        [Required]
        public int UserUUID { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
