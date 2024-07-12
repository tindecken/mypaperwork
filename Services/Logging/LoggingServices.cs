using System.Security.Cryptography;
using System.Text;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Models.Logging;
using mypaperwork.Utils;
using Serilog;
using SQLite;

namespace mypaperwork.Services.Logging;
public class LoggingServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly SQLiteAsyncConnection _sqliteDb;
    public LoggingServices(IHttpContextAccessor httpContextAccessor, SQLiteAsyncConnection sqliteDb, AppSettings appSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _sqliteDb = sqliteDb;
        _appSettings = appSettings;
    }
    public async Task<GenericResponseData> AddLog(LoggingDTO loggingDTO)
    {
        var responseData = new GenericResponseData();

        var httpContextUtils = new HttpContextUtils(_httpContextAccessor);
        var userGUID = httpContextUtils.getUserGUID();
        var ipAddress = httpContextUtils.getClientIPAddress();
        var logging = new Logs()
        {
            GUID = Guid.NewGuid().ToString(),
            ActionType = loggingDTO.ActionType,
            Method = loggingDTO.Method,
            Message = loggingDTO.Message,
            OldData = loggingDTO.OldData,
            NewData = loggingDTO.NewData,
            ActionBy = string.IsNullOrEmpty(userGUID) ? "Not specified" : userGUID,
            IPAddress = ipAddress
        };
        await _sqliteDb.InsertAsync(logging);
        var logs = await _sqliteDb.Table<Logs>().ToListAsync();
        Log.Information(logs.ToString());
        responseData.Data = logs;
        responseData.Success = true;
        responseData.Message = "Successfully added a log";
        return responseData;
    }
    
}