using System.Net;
using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Models.Database;
using mypaperwork.Models.Files;
using mypaperwork.Models.Filter;
using mypaperwork.Services.Logging;
using mypaperwork.Utils;
using SQLite;

namespace mypaperwork.Services.FileServices;
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

    public async Task<GenericResponseData> SelectFile(string fileId)
    {
        var responseData = new GenericResponseData();
        var token = _httpContextUtils.GetToken();
        var userId = token.userId;
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == fileId && f.IsDeleted == 0).FirstOrDefaultAsync();
        if (file == null ) { 
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"File {fileId} not found or was deleted.";
            responseData.Success = true;
            return responseData;
        }
        var userfileId = await _dbUtils.GetSelectedUsersFilesId(userId, fileId);
        if (string.IsNullOrEmpty(userfileId)){ 
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Success = false;
            responseData.Message = "There's no file associated.";
            return responseData;
        }
        await _dbUtils.SetSelectedFileAsync(userId, fileId);
        var user = await _sqliteDb.Table<Users>().Where(u => u.Id == userId).FirstOrDefaultAsync();
        var selectedUserFile = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserId == userId && uf.FileId == fileId).FirstOrDefaultAsync();
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
        var userId = token.userId;
        
        // check existed file
        var existedFile = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Name == model.Name && f.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedFile != null)
        {
            var existedUsersFiles = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserId == userId && uf.FileId == existedFile.Id && uf.IsDeleted == 0).FirstOrDefaultAsync();
            if (existedUsersFiles != null)
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Message = $"File {model.Name.Trim()} is duplicated.";
                return responseData;
            }
        }
        
        var file = new FilesDBModel
        {
            Id = Ulid.NewUlid().ToString(),
            Name = model.Name.Trim(),
            Description = model.Description,
            CreatedBy = userId,
            IsDeleted = 0
        };
        await _sqliteDb.InsertAsync(file);
        var userfile = new UsersFiles
        {
            Id = Ulid.NewUlid().ToString(),
            UserId = userId,
            FileId = file.Id,
            Role = "Admin"
        };
        await _sqliteDb.InsertAsync(userfile);
        await _dbUtils.SetSelectedFileAsync(userId, file.Id);
        // Regenerate token
        var user = await _sqliteDb.Table<Users>().Where(u => u.Id == userId).FirstOrDefaultAsync();
        var reGeneratedtoken = _jwtUtils.generateJwtToken(user, userfile);
        var response = new AuthenticateResponse(user, reGeneratedtoken);
        responseData.Data = response;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Create file: {file.Name} successfully.";
        responseData.Data = reGeneratedtoken;
        responseData.Success = true;
        return responseData;
    }
    public async Task<GenericResponseData> DeleteFile(string fileId)
    {
        var responseData = new GenericResponseData();
        var token = _httpContextUtils.GetToken();
        var userId = token.userId;
        
        // check existed file
        var existedFile = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == fileId && f.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedFile == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {fileId} is not existed.";
            return responseData;
        }
        var existedUserFile = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserId == userId && uf.FileId == fileId && uf.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedUserFile == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {fileId} is not associated with user {userId}.";
            return responseData;
        }
        // update file
        existedFile.IsDeleted = 1;
        existedFile.UpdatedBy = userId;
        existedFile.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(existedFile);
        
        // update userfile
        existedUserFile.IsDeleted = 1;
        existedUserFile.IsSelected = 0;
        existedUserFile.UpdatedBy = userId;
        existedUserFile.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(existedUserFile);
        
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Delete file: {fileId} successfully.";
        responseData.Success = true;
        return responseData;
    }

    public async Task<GenericResponseData> GetFilesByUser()
    {
        var responseData = new GenericResponseData();
        var token = _httpContextUtils.GetToken();
        var userId = token.userId;
        var userFiles = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserId == userId && uf.IsDeleted == 0)
            .ToListAsync();
        var files = new List<FilesDBModel>();

        foreach (var userFile in userFiles)
        {
            var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == userFile.FileId && f.IsDeleted == 0)
                .FirstOrDefaultAsync();
            if (file != null) files.Add(file);
        }

        responseData.Data = files;
        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Get {files.Count} files successfully.";
        responseData.TotalRecords = files.Count;
        responseData.Success = true;
        return responseData;
    }
}