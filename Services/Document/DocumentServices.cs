using System.Security.Cryptography;
using System.Text;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Utils;
using Serilog;
using SQLite;

namespace mypaperwork.Services.Document;
public class DocumentServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly SQLiteAsyncConnection _sqliteDb;
    public DocumentServices(IHttpContextAccessor httpContextAccessor, SQLiteAsyncConnection sqliteDb, AppSettings appSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _sqliteDb = sqliteDb;
        _appSettings = appSettings;
    }
    public async Task<GenericResponseData> UploadDocument()
    {
                
    };
    
}