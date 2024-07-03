using System.Net;
using System.Security.Cryptography;
using System.Text;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Services.Logging;
using Serilog;
using SQLite;

namespace mypaperwork.Services.Files;
public class FilesServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly LoggingServices _loggingServices;
    private readonly SQLiteAsyncConnection _sqliteDb;
    public FilesServices(IHttpContextAccessor httpContextAccessor, AppSettings appSettings, LoggingServices loggingServices, SQLiteAsyncConnection sqliteDb)
    {
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings;
        _loggingServices = loggingServices;
        _sqliteDb = sqliteDb;
    }

    public async Task<GenericResponseData> SelectFile(string fileGUID)
    {
        var responseData = new GenericResponseData();

        return responseData;
    }
    
}