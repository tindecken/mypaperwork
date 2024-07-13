using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Models.Database;
using mypaperwork.Models.Logging;
using mypaperwork.Services.Logging;
using mypaperwork.Utils;
using Newtonsoft.Json;
using Serilog;
using SQLite;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace mypaperwork.Services.User;
public class UserServices
{
    private readonly string _JWTsecret;
    private readonly JWTUtils _jwtUtils;
    private readonly LoggingServices _loggingServices;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SQLiteAsyncConnection _sqliteDb;
    private readonly DBUtils _dbUtils;
    public UserServices(AppSettings appSettings, JWTUtils jwtUtils, LoggingServices loggingServices, IHttpContextAccessor httpContextAccessor, SQLiteAsyncConnection sqliteDb, DBUtils dbUtils)
    {
        _JWTsecret = appSettings.JWTSecret;
        _jwtUtils = jwtUtils;
        _loggingServices = loggingServices;
        _httpContextAccessor = httpContextAccessor;
        _sqliteDb = sqliteDb;
        _dbUtils = dbUtils;
    }
    public async Task<GenericResponseData> Authenticate(AuthenticateRequestModel model)
    {
        Log.Information($"AuthenticateRequestModel: {JsonConvert.SerializeObject(model)}");
        var responseData = new GenericResponseData();
        var user = await _sqliteDb.Table<Users>().Where(u => u.UserName == model.UserName).FirstOrDefaultAsync();
        // return null if user not found
        if (user == null) return new GenericResponseData()
        {
            Success = false,
            Message = "Incorrect username or password!",
            StatusCode = HttpStatusCode.Unauthorized,
            Data = null,
            Error = null
        };
        if (user.IsDeleted == 1) { 
            responseData.Success = false;
            responseData.Message = "User is disabled, please contact administration!";
            responseData.StatusCode = HttpStatusCode.Unauthorized;
            _ = _loggingServices.AddLog(new LoggingDTO()
            {
                ActionType = ActionType.LoginFailed.ToString(),
                Method = "Authenticate",
                Message = $"User is disabled: {user.UserName}",
            });
            return responseData;
        }

        byte[] data = Convert.FromBase64String(user.Password);
        MD5 md5 = MD5.Create();
        var tripDes = TripleDES.Create();
        tripDes.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_JWTsecret));
        tripDes.Mode = CipherMode.ECB;
        tripDes.Padding = PaddingMode.PKCS7;

        ICryptoTransform transform = tripDes.CreateDecryptor();
        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
        var decryptedPassword = Encoding.UTF8.GetString(results);

        if (decryptedPassword.Equals(model.Password))
        {
            // TODO: Get all files associated with users
            var userFiles = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserGUID == user.GUID && uf.IsDeleted == 0).ToListAsync();
            
            // filter deleted file
            var userFilesWithoutDeletedFiles = new List<UsersFiles>();
            foreach (var userFile in userFiles)
            {
                var file = await _sqliteDb.Table<Models.Database.FilesDBModel>().Where(f => f.GUID == userFile.FileGUID && f.IsDeleted == 0).FirstOrDefaultAsync();
                if (file != null) userFilesWithoutDeletedFiles.Add(userFile);
            }
            // selectedUserFile
            var selectedUserFile = userFilesWithoutDeletedFiles.FirstOrDefault(u => u.IsSelected != 0);

            // authentication successful so generate jwt token
            var token = _jwtUtils.generateJwtToken(user, selectedUserFile);
            var response = new AuthenticateResponse(user, token);
            responseData.Data = response;
            responseData.Success = true;
            responseData.Message = "Login successfully !";
            _ = _loggingServices.AddLog(new LoggingDTO()
            {
                ActionType = ActionType.LoginSuccess.ToString(),
                Method = "Authenticate",
                Message = $"Login success with user: {user.UserName}",
            });
        }
        else
        {
            responseData.Success = false;
            responseData.Message = "Incorrect username or password!";
            responseData.StatusCode = HttpStatusCode.BadRequest;
            _ = _loggingServices.AddLog(new LoggingDTO()
            {
                ActionType = ActionType.LoginFailed.ToString(),
                Method = "Authenticate",
                Message = $"Login failed with model: {JsonConvert.SerializeObject(model)}",
            });
        }
        return responseData;
    }

    public async Task<GenericResponseData> ChangePassword(ChangePasswordRequestModel changePasswordData)
    {
        var responseData = new GenericResponseData();
        //    // Get regression test
        //    // Check New Password and Confirm Password is the same
        //    if (!changePasswordData.NewPassword.Equals(changePasswordData.ConfirmNewPassword))
        //    {
        //        responseData.Success = false;
        //        responseData.Message = "New password and confirm password are not the same!";
        //        responseData.StatusCode = HttpStatusCode.BadRequest;
        //        return responseData;
        //    }

        //    // Check if CurrentPassword is the same with new Password
        //    if (changePasswordData.CurrentPassword.Equals(changePasswordData.NewPassword))
        //    {
        //        responseData.Success = false;
        //        responseData.Message = "Current password and new password are the same!";
        //        responseData.StatusCode = HttpStatusCode.BadRequest;
        //        return responseData;
        //    }
        //    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == changePasswordData.UserGUID);
        //    if (user == null)
        //    {
        //        responseData.Success = false;
        //        responseData.Message = "Users not found!";
        //        responseData.StatusCode = HttpStatusCode.BadRequest;
        //        return responseData;
        //    }

        //    //Check current password is correct or not
        //    byte[] data = Convert.FromBase64String(user.Password);
        //    MD5 md5 = MD5.Create();
        //    var tripDes = TripleDES.Create();
        //    tripDes.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_JWTsecret));
        //    tripDes.Mode = CipherMode.ECB;

        //    ICryptoTransform transform = tripDes.CreateDecryptor();
        //    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
        //    var decryptedPassword = Encoding.UTF8.GetString(results);

        //    if (!decryptedPassword.Equals(changePasswordData.CurrentPassword))
        //    {
        //        responseData.Success = false;
        //        responseData.Message = "Current password is incorrect!";
        //        responseData.StatusCode = HttpStatusCode.BadRequest;
        //        return responseData;
        //    }

        //    // Change password
        //    // Encrypt new password
        //    var tripDes2 = TripleDES.Create();
        //    tripDes2.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_JWTsecret));
        //    tripDes2.Mode = CipherMode.ECB;
        //    tripDes2.Padding = PaddingMode.PKCS7;
        //    byte[] DataToEncrypt = UTF8Encoding.UTF8.GetBytes(changePasswordData.NewPassword);
        //    ICryptoTransform cTransform = tripDes2.CreateEncryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);

        //    var userUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == changePasswordData.UserGUID);
        //    userUpdate.Password = Convert.ToBase64String(resultArray);
        //    await _dbContext.SaveChangesAsync();

        //    responseData.Success = true;
        //    responseData.Message = "Change password successfully!";
        //    responseData.StatusCode = HttpStatusCode.OK;

        //    _loggingServiceses.AddLog(new LogDTO()
        //    {
        //        ActionType = ActionType.ChangePassword.ToString(),
        //        Message = $"Users {user.UserName} change password successfully!",
        //        Method = "Authenticate",
        //    });

        //    return responseData;
        //}
        //public async Task<GenericResponseData> CreateUser(CreateUserRequestModel createUserRequestModel)
        //{
        //    var emailUtils = new EmailUtils();
        //    var httpContextUtils = new HttpContextUtils(_httpContextAccessor);
        //    var responseData = new GenericResponseData();
        //    // Check email is valid or not
        //    if (!emailUtils.isEmailValid(createUserRequestModel.Email))
        //    {
        //        responseData.Success = false;
        //        responseData.Message = "Email is not valid!";
        //        responseData.StatusCode = HttpStatusCode.BadRequest;
        //        return responseData;
        //    }

        //    // Check userName is exist or not
        //    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == createUserRequestModel.UserName);
        //    if (user != null)
        //    {
        //        responseData.Success = false;
        //        responseData.Message = "Users name is exist!";
        //        responseData.StatusCode = HttpStatusCode.BadRequest;
        //        return responseData;
        //    }

        //    // check email is exist or not
        //    var emailUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == createUserRequestModel.Email);
        //    if (emailUser != null)
        //    {
        //        responseData.Success = false;
        //        responseData.Message = "Email is exist!";
        //        responseData.StatusCode = HttpStatusCode.BadRequest;
        //        return responseData;
        //    }

        //    // Encrypt password
        //    MD5 md5 = MD5.Create();
        //    var tripDes2 = TripleDES.Create();
        //    tripDes2.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_JWTsecret));
        //    tripDes2.Mode = CipherMode.ECB;
        //    tripDes2.Padding = PaddingMode.PKCS7;
        //    byte[] DataToEncrypt = UTF8Encoding.UTF8.GetBytes(createUserRequestModel.Password);
        //    ICryptoTransform cTransform = tripDes2.CreateEncryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);

        //    var createUser = _mapper.Map<Users>(createUserRequestModel);
        //    createUser.CreatedOn = DateTime.UtcNow.ToString("u");;
        //    createUser.CreatedById = httpContextUtils.getUserGUID();
        //    createUser.Password = Convert.ToBase64String(resultArray);
        //    var a = await _dbContext.Users.AddAsync(createUser);
        //    await _dbContext.SaveChangesAsync();

        //    responseData.Success = true;
        //    responseData.Message = "Create user successfully!";
        //    responseData.StatusCode = HttpStatusCode.OK;

        return responseData;
    }
}