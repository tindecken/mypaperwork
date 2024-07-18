using System.Net;
using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Models.Database;
using mypaperwork.Models.Files;
using mypaperwork.Models.Filter;
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
    public async Task<GenericResponseData> CreateFile(CreateFileRequestModel model)
    {
        var responseData = new GenericResponseData();
        var token = _httpContextUtils.GetToken();
        var userGUID = token.userGUID;
        
        // check existed file
        var existedFile = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Name == model.Name && f.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedFile != null)
        {
            var existedUsersFiles = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserGUID == userGUID && uf.FileGUID == existedFile.GUID && uf.IsDeleted == 0).FirstOrDefaultAsync();
            if (existedUsersFiles != null)
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Message = $"File {model.Name.Trim()} is duplicated.";
                return responseData;
            }
        }
        
        var file = new FilesDBModel
        {
            GUID = Guid.NewGuid().ToString(),
            Name = model.Name.Trim(),
            Description = model.Description,
            CreatedBy = userGUID,
            IsDeleted = 0
        };
        await _sqliteDb.InsertAsync(file);
        var userfile = new UsersFiles
        {
            GUID = Guid.NewGuid().ToString(),
            UserGUID = userGUID,
            FileGUID = file.GUID,
            Role = "Admin"
        };
        await _sqliteDb.InsertAsync(userfile);
        await _dbUtils.SetSelectedFileAsync(userGUID, file.GUID);
        // Regenerate token
        var user = await _sqliteDb.Table<Users>().Where(u => u.GUID == userGUID).FirstOrDefaultAsync();
        var reGeneratedtoken = _jwtUtils.generateJwtToken(user, userfile);
        var response = new AuthenticateResponse(user, reGeneratedtoken);
        responseData.Data = response;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Create file: {file.Name} successfully.";
        responseData.Data = reGeneratedtoken;
        responseData.Success = true;
        return responseData;
    }
    public async Task<GenericResponseData> DeleteFile(string fileGUID)
    {
        var responseData = new GenericResponseData();
        var token = _httpContextUtils.GetToken();
        var userGUID = token.userGUID;
        
        // check existed file
        var existedFile = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == fileGUID && f.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedFile == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {fileGUID} is not existed.";
            return responseData;
        }
        var existedUserFile = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserGUID == userGUID && uf.FileGUID == fileGUID && uf.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedUserFile == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {fileGUID} is not associated with user {userGUID}.";
            return responseData;
        }
        // update file
        existedFile.IsDeleted = 1;
        existedFile.UpdatedBy = userGUID;
        existedFile.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(existedFile);
        
        // update userfile
        existedUserFile.IsDeleted = 1;
        existedUserFile.IsSelected = 0;
        existedUserFile.UpdatedBy = userGUID;
        existedUserFile.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(existedUserFile);
        
        responseData.Data = null;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Delete file: {fileGUID} successfully.";
        responseData.Success = true;
        return responseData;
    }

    public async Task<GenericResponseData> GetFilesByUser(PaginationFilter filter)
    {
        var responseData = new GenericResponseData();
        var token = _httpContextUtils.GetToken();
        var userGUID = token.userGUID;
        var userFiles = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserGUID == userGUID && uf.IsDeleted == 0)
            .ToListAsync();
        var files = new List<FilesDBModel>();

        foreach (var userFile in userFiles)
        {
            var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == userFile.FileGUID && f.IsDeleted == 0)
                .FirstOrDefaultAsync();
            if (file != null)
            {
                files.Add(file);
            }
        }

        responseData.Data = files;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Get {files.Count} files successfully.";
        responseData.TotalRecords = files.Count;
        responseData.Success = true;
        return responseData;
    }
}