using mypaperwork.Models.Database;

namespace mypaperwork.Models.Authentication;

public class AuthenticateResponse
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public UserRole SystemRole { get; set; }
    public string UserName { get; set; }
    public string Token { get; set; }
    public AuthenticateResponse(Users users, string resToken)
    {
        Id = users.Id;
        Email = users.Email;
        Name = users.Name;
        SystemRole = (UserRole)Enum.Parse(typeof(UserRole), users.SystemRole);
        UserName = users.UserName;
        Token = resToken;
    }
}