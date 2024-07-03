using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using mypaperwork.Models.Database;
using mypaperwork.Services.User;

namespace mypaperwork.Utils;

public class JWTUtils
{
    private readonly AppSettings _appSettings;
    public JWTUtils(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }
    public string generateJwtToken(Users users, UsersFiles usersFiles)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.JWTSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] 
            { new Claim("userGUID", users.GUID.ToString()),
                new Claim("systemRole", users.SystemRole),
                new Claim("email", users.Email),
                usersFiles != null ? new Claim("selectedFileGUID", usersFiles.FileGUID.ToString()) : new Claim("selectedFileGUID", ""),
                usersFiles != null ? new Claim("selectedFileRole", usersFiles.Role.ToString()) : new Claim("selectedFileRole", ""),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    /// <summary>
    /// Claim Users information from token, then add it into HttpContext, that can be retrieved Users information by HttpContext.Items["Users"]
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <param name="userService">UserService</param>
    /// <param name="token">jwt token</param>
    public void attachUserToContext(HttpContext context, UserServices userService, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JWTSecret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userGUID = jwtToken.Claims.First(x => x.Type == "userGUID").Value;

            // attach user to context on successful jwt validation
            context.Items["Users"] = userService.GetById(userGUID);
        }
        catch (Exception ex)
        {
            Log.Error("attachUserToContext: {0}", ex.Message);
            // do nothing if jwt validation fails
            // user is not attached to context so request won't have access to secure routes
        }
    }
}