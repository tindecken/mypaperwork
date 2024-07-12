using System.Net;
using mypaperwork.Models;
using mypaperwork.Models.Authentication;
using mypaperwork.Models.Database;
using mypaperwork.Services.Logging;
using mypaperwork.Utils;
using SQLite;

namespace mypaperwork.Services.Category;
public class CategoryServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly LoggingServices _loggingServices;
    private readonly SQLiteAsyncConnection _sqliteDb;
    private readonly HttpContextUtils _httpContextUtils;
    private readonly DBUtils _dbUtils;
    private readonly JWTUtils _jwtUtils;

    public CategoryServices(HttpContextUtils httpContextUtils, AppSettings appSettings, LoggingServices loggingServices, SQLiteAsyncConnection sqliteDb, DBUtils dbUtils, JWTUtils jwtUtils)
    {
        _httpContextUtils = httpContextUtils;
        _appSettings = appSettings;
        _loggingServices = loggingServices;
        _sqliteDb = sqliteDb;
        _dbUtils = dbUtils;
        _jwtUtils = jwtUtils;
    }

    public async Task<GenericResponseData> CreateNewCategory(Categories cat)
    {
        var responseData = new GenericResponseData();
        
        
        return responseData;
    }
    
}