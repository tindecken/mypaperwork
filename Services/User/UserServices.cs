﻿using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Models.Database;
using mypaperwork.Services.Logging;
using mypaperwork.Utils;

namespace mypaperwork.Services.User;
public class UserServices
{
    private readonly string _JWTsecret;
    private readonly JWTUtils _jwtUtils;
    private readonly LoggingServices _loggingServiceses;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserServices(AppSettings appSettings, JWTUtils jwtUtils, LoggingServices loggingServiceses, IHttpContextAccessor httpContextAccessor)
    {
        _JWTsecret = appSettings.JWTSecret;
        _jwtUtils = jwtUtils;
        _loggingServiceses = loggingServiceses;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<GenericResponseData> Authenticate(AuthenticateRequestModel model)
    {
        //Logs.Information($"AuthenticateRequestModel: {JsonConvert.SerializeObject(model)}");
        var responseData = new GenericResponseData();
        //var user = _dbContext.Users.FirstOrDefault(u => u.UserName == model.UserName);
        //// return null if user not found
        //if (user == null) return new GenericResponseData()
        //    {
        //        Success = false,
        //        Message = $"Not found user: {model.UserName} !",
        //        StatusCode = HttpStatusCode.Unauthorized,
        //        Data = null,
        //        Error = null
        //    };
        //byte[] data = Convert.FromBase64String(user.Password);
        //MD5 md5 = MD5.Create();
        //var tripDes = TripleDES.Create();
        //tripDes.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_JWTsecret));
        //tripDes.Mode = CipherMode.ECB;

        //ICryptoTransform transform = tripDes.CreateDecryptor();
        //byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
        //var decryptedPassword = Encoding.UTF8.GetString(results);

        //if (decryptedPassword.Equals(model.Password))
        //{
        //    // authentication successful so generate jwt token
        //    var token = _jwtUtils.generateJwtToken(user);
        //    Logs.Information("Token: {0}", token);
        //    var response = new AuthenticateResponse(user, token);
        //    responseData.Data = response;
        //    responseData.Success = true;
        //    responseData.Message = "Login successfully !";
        //    _loggingServiceses.AddLog(new LogDTO()
        //    {
        //        ActionType = ActionType.LoginSuccess.ToString(),
        //        Method = "Authenticate",
        //        Message = $"Login success with user: {user.UserName}",
        //    });
        //}
        //else
        //{
        //    responseData.Success = false;
        //    responseData.Message = "Incorrect password !";
        //    responseData.StatusCode = HttpStatusCode.BadRequest;
        //    _loggingServiceses.AddLog(new LogDTO()
        //    {
        //        ActionType = ActionType.LoginFailed.ToString(),
        //        Method = "Authenticate",
        //        Message = $"Login failed with model: {JsonConvert.SerializeObject(model)}",
        //    });
        //}
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
    //    var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == changePasswordData.UserUUID);
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

    //    var userUpdate = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == changePasswordData.UserUUID);
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
    //    createUser.CreatedOn = DateTime.Now;
    //    createUser.CreatedById = httpContextUtils.getUserUUID();
    //    createUser.Password = Convert.ToBase64String(resultArray);
    //    var a = await _dbContext.Users.AddAsync(createUser);
    //    await _dbContext.SaveChangesAsync();
        
    //    responseData.Success = true;
    //    responseData.Message = "Create user successfully!";
    //    responseData.StatusCode = HttpStatusCode.OK;
        
        return responseData;
    }

    public Users GetById(string id)
    {
        // TODO: Implement GetById
        return new Users()
        {

        };
        //return _dbContext.Users.FirstOrDefault(u => u.Id.ToString() == id);
    }
}