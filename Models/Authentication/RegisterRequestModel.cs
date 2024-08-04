using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Authentication;

public class RegisterRequestModel
{
    [Required]
    [MinLength(3), MaxLength(50)]
    public string UserName { get; set; }
    [MinLength(2), MaxLength(200)]
    public string Name { get; set; }
    [Required]
    [MinLength(3), MaxLength(50)]
    public string Password { get; set; }
    [Required]
    [MinLength(3), MaxLength(50)]
    public string ConfirmPassword { get; set; }
    [EmailAddress]
    [Required]
    [MinLength(3), MaxLength(50)]
    public string Email { get; set; }
}