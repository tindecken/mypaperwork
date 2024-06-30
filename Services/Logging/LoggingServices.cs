using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Utils;
using Serilog;
using SQLite;

namespace mypaperwork.Services.Logging;
public class LoggingServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SQLiteAsyncConnection _sqliteDb;
    public LoggingServices(IHttpContextAccessor httpContextAccessor, SQLiteAsyncConnection sqliteDb)
    {
        _httpContextAccessor = httpContextAccessor;
        _sqliteDb = sqliteDb;
    }
    public async Task<GenericResponseData> AddLog()
    {
        var responseData = new GenericResponseData();

        var httpContextUtils = new HttpContextUtils(_httpContextAccessor);
        var userGUID = httpContextUtils.getUserGUID();
        var ipAddress = httpContextUtils.getClientIPAddress();
        var logging = new Logs()
        {
            GUID = Guid.NewGuid().ToString()
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