using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
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
    public async Task<GenericResponseData> Upload(string paperWorkGUID, IFormFileCollection files)
    {
        var responseData = new GenericResponseData();

        var storagePath = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.StoragePath);
        foreach (var file in files)
        {
            var fileSize = file.Length;
            var fileNameFull = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
            var fileName = new string((Path.GetFileNameWithoutExtension(fileNameFull) ?? string.Empty).Take(20).ToArray()).Replace(" ", "-");
            fileName = fileName + Guid.NewGuid() + Path.GetExtension(fileNameFull);
            var sourceFile = Path.Combine(storagePath, fileName);
            await using (var stream = new FileStream(sourceFile, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Documents()
            {
                GUID = Guid.NewGuid().ToString(),
                PaperWorkGUID = paperWorkGUID,
                FileName = fileName,
                FileSize = fileSize,
            };
            await _sqliteDb.InsertAsync(document);
        }

        responseData.Data = null;
        responseData.Success = true;
        responseData.Message = $"Successfully added {files.Count} document(s) for paperwork: {paperWorkGUID}.";
        responseData.TotalRecords = files.Count;
        return responseData;
    }
}