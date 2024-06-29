using mypaperwork.Models.Database;

namespace mypaperwork.Models.Authentication;

public class AuthenticateResponse
{
    public string UUID { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public string UserName { get; set; }
    public string Token { get; set; }
    public AuthenticateResponse(Users users, string resToken)
    {
        UUID = users.UUID;
        Email = users.Email;
        Name = users.Name;
        Role = (UserRole)Enum.Parse(typeof(UserRole), users.Role);;
        UserName = users.UserName;
        Token = resToken;
    }
}