using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Authentication;

public class AuthenticateRequestModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}