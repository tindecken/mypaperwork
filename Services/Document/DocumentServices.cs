using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Models.Filter;
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
    public async Task<GenericResponseData> Upload(string paperWorkId, IFormFileCollection files)
    {
        var responseData = new GenericResponseData();
        
        // check user is selected file or not
        var selectedFileId = _httpContextUtils.GetSelectedFileId();
        if (string.IsNullOrEmpty(selectedFileId))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check file is existed or not
        var existedFile = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (existedFile == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileId} not found or deleted";
            return responseData;
        }
        // check exist paperwork
        var existedPaperwork = await _sqliteDb.Table<Paperworks>().Where(c => c.Id == paperWorkId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedPaperwork == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Paperwork {paperWorkId}  not found or was deleted";
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
        var documents = new List<Documents>();
        foreach (var file in files)
        {
            var fileSize = file.Length;
            var fileNameFull = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
            var fileName = new string((Path.GetFileNameWithoutExtension(fileNameFull) ?? string.Empty).Take(20).ToArray()).Replace(" ", "-");
            fileName = fileName + Ulid.NewUlid() + Path.GetExtension(fileNameFull);
            // create folder if not exist
            if (!Directory.Exists(Path.Combine(storagePath, existedFile.Id)))
            {
                Directory.CreateDirectory(Path.Combine(storagePath, existedFile.Id));
            }

            var sourceFile = Path.Combine(storagePath, existedFile.Id, fileName);
            await using (var stream = new FileStream(sourceFile, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Documents()
            {
                Id = Ulid.NewUlid().ToString(),
                PaperWorkId = paperWorkId,
                Folder = existedFile.Id,
                FileName = fileName,
                FileSize = fileSize,
                CreatedBy = _httpContextUtils.GetUserId()
            };
            await _sqliteDb.InsertAsync(document);
            documents.Add(document);
        }

        responseData.Data = documents;
        responseData.Success = true;
        responseData.Message = $"Successfully added {files.Count} document(s) for paperwork: {existedPaperwork.Name}.";
        responseData.TotalRecords = files.Count;
        return responseData;
    }
    public async Task<GenericResponseData> GetDocumentById(string documentId)
    {
        var responseData = new GenericResponseData();
        
        // check user is selected file or not
        var selectedFileId = _httpContextUtils.GetSelectedFileId();
        if (string.IsNullOrEmpty(selectedFileId))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check file is existed or not
        var existedFile = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (existedFile == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileId} not found or deleted";
            return responseData;
        }
        
        var document = await _sqliteDb.Table<Documents>().Where(d => d.Id == documentId && d.IsDeleted == 0).FirstOrDefaultAsync();
        if (document == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Document {documentId} not found or was deleted";
            return responseData;
        }
        
        // get file path
        var filePath = Path.Combine(_appSettings.StoragePath, existedFile.Id, document.FileName);
        if (File.Exists(filePath))
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var mimeType = GetMimeType(filePath);
            
            responseData.StatusCode = HttpStatusCode.OK;
            responseData.Data = new { Content = fileBytes, MimeType = mimeType, document };
            responseData.Success = true;
            responseData.Message = $"Retrieve document success";
            return responseData;
        }
        else
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"File {document.FileName} not found";
            return responseData;
        }
    }
    private string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream",
        };
    }
}