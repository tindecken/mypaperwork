using System.Net;
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
    private readonly HttpContextUtils _httpContextUtils;
    public DocumentServices(IHttpContextAccessor httpContextAccessor, SQLiteAsyncConnection sqliteDb, AppSettings appSettings, HttpContextUtils httpContextUtils)
    {
        _httpContextAccessor = httpContextAccessor;
        _sqliteDb = sqliteDb;
        _appSettings = appSettings;
        _httpContextUtils = httpContextUtils;
    }
    public async Task<GenericResponseData> Upload(string paperWorkGUID, IFormFileCollection files)
    {
        var responseData = new GenericResponseData();
        
        // check user is selected file or not
        var selectedFileGUID = _httpContextUtils.GetSelectedFileGUID();
        if (string.IsNullOrEmpty(selectedFileGUID))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check file is existed or not
        var existedFile = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == selectedFileGUID && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (existedFile == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileGUID} not found or deleted";
            return responseData;
        }
        // check exist paperwork
        var existedPaperwork = await _sqliteDb.Table<Paperworks>().Where(c => c.GUID == paperWorkGUID && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedPaperwork == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Paperwork {paperWorkGUID}  not found or was deleted";
            return responseData;
        }

        var storagePath = Path.Combine(Directory.GetCurrentDirectory(), _appSettings.StoragePath);
        foreach (var file in files) { 
            var fileSize = file.Length;
            if (fileSize > _appSettings.MaxFileSize) {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Data = null;
                responseData.Success = false;
                responseData.Message = $"File size of file {ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName} is too large ({fileSize}/{_appSettings.MaxFileSize} bytes).";
                return responseData;
            }
            else continue;
        }
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
        responseData.Message = $"Successfully added {files.Count} document(s) for paperwork: {existedPaperwork.Name}.";
        responseData.TotalRecords = files.Count;
        return responseData;
    }
}