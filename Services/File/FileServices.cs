using System.Net;
using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Models.Database;
using mypaperwork.Services.Logging;
using mypaperwork.Utils;
using SQLite;

namespace mypaperwork.Services.File;
public class FileServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly LoggingServices _loggingServices;
    private readonly SQLiteAsyncConnection _sqliteDb;
    private readonly HttpContextUtils _httpContextUtils;
    private readonly DBUtils _dbUtils;
    private readonly JWTUtils _jwtUtils;

    public FileServices(HttpContextUtils httpContextUtils, AppSettings appSettings, LoggingServices loggingServices, SQLiteAsyncConnection sqliteDb, DBUtils dbUtils, JWTUtils jwtUtils)
    {
        _httpContextUtils = httpContextUtils;
        _appSettings = appSettings;
        _loggingServices = loggingServices;
        _sqliteDb = sqliteDb;
        _dbUtils = dbUtils;
        _jwtUtils = jwtUtils;
    }

    public async Task<GenericResponseData> SelectFile(string fileGUID)
    {
        var responseData = new GenericResponseData();
        var token = _httpContextUtils.GetToken();
        var userGUID = token.userGUID;
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == fileGUID && f.IsDeleted == 0).FirstOrDefaultAsync();
        if (file == null ) { 
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"File {fileGUID} not found or was deleted.";
            responseData.Success = true;
            return responseData;
        }
        var userfileGUID = await _dbUtils.GetSelectedUsersFilesGUID(userGUID, fileGUID);
        if (string.IsNullOrEmpty(userfileGUID)){ 
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Success = false;
            responseData.Message = "There's no file associated.";
            return responseData;
        }
        await _dbUtils.SetSelectedFileAsync(userGUID, fileGUID);
        var user = await _sqliteDb.Table<Users>().Where(u => u.GUID == userGUID).FirstOrDefaultAsync();
        var selectedUserFile = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserGUID == userGUID && uf.FileGUID == fileGUID).FirstOrDefaultAsync();
        // Regenerate token
        var reGeneratedtoken = _jwtUtils.generateJwtToken(user, selectedUserFile);
        var response = new AuthenticateResponse(user, reGeneratedtoken);
        responseData.Data = response;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Select file: {file.Name} successfully.";
        responseData.Data = reGeneratedtoken;
        responseData.Success = true;
        return responseData;
    }
    
}