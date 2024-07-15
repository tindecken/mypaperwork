using System.Net;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Models.Paperwork;
using mypaperwork.Services.Logging;
using mypaperwork.Utils;
using SQLite;

namespace mypaperwork.Services.Paperwork;

public class PaperworkServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly LoggingServices _loggingServices;
    private readonly SQLiteAsyncConnection _sqliteDb;
    private readonly HttpContextUtils _httpContextUtils;
    private readonly DBUtils _dbUtils;

    public PaperworkServices(HttpContextUtils httpContextUtils, AppSettings appSettings, LoggingServices loggingServices, SQLiteAsyncConnection sqliteDb, DBUtils dbUtils)
    {
        _httpContextUtils = httpContextUtils;
        _appSettings = appSettings;
        _loggingServices = loggingServices;
        _sqliteDb = sqliteDb;
        _dbUtils = dbUtils;
    }
    public async Task<GenericResponseData> CreatePaperwork(CreatePaperworkRequestModel model)
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
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == selectedFileGUID && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileGUID} not found or deleted";
            return responseData;
        }
        // check category (with file) is exist or not
        var existedCategory = await _sqliteDb.Table<Categories>().Where(c => c.GUID == model.CategoryGUID && c.FileGUID == selectedFileGUID && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedCategory == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category {model.CategoryGUID} associated with file {selectedFileGUID} not found or was deleted";
            return responseData;
        }
        // create new paperwork
        var paperwork = new Paperworks()
        {
            GUID = Guid.NewGuid().ToString(),
            Name = model.Name,
            Description = model.Description,
            IssuedDate = model.IssuedDate,
            Price = model.Price,
            PriceCurrency = model.PriceCurrency,
            CreatedBy = _httpContextUtils.GetUserGUID(),
        };
        var result = await _sqliteDb.InsertAsync(paperwork);
        if (result <= 0)
        {
            responseData.StatusCode = HttpStatusCode.InternalServerError;
            responseData.Message = $"Something went wrong while creating new paperwork: {model.Name}";
            responseData.Data = paperwork;
            return responseData;
        }
        // create associate between category and paperwork
        var categoryPaperwork = new PaperworksCategories()
        {
            GUID = Guid.NewGuid().ToString(),
            CategoryGUID = model.CategoryGUID,
            PaperworkGUID = paperwork.GUID,
            CreatedBy = _httpContextUtils.GetUserGUID(),
        };
        result = await _sqliteDb.InsertAsync(categoryPaperwork);
        if (result <= 0)
        {
            responseData.StatusCode = HttpStatusCode.InternalServerError;
            responseData.Message = $"Something went wrong while creating new association between category and paperwork";
            responseData.Data = paperwork;
            return responseData;
        }
        responseData.StatusCode = HttpStatusCode.Created;
        responseData.Message = $"Paperwork {paperwork.Name} created successfully";
        responseData.Data = paperwork;
        return responseData;
    }
    public async Task<GenericResponseData> UpdatePaperwork(UpdatePaperworkRequestModel model)
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
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == selectedFileGUID && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileGUID} not found or deleted";
            return responseData;
        }
        
        // check paperwork is exist or not
        var existedPaperwork = await _sqliteDb.Table<Paperworks>().Where(p => p.GUID == model.GUID && p.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedPaperwork == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Paperwork {model.GUID} not found or was deleted";
            return responseData;
        }
        
        // update existed paperwork
        if (!string.IsNullOrEmpty(model.Name)) existedPaperwork.Name = model.Name;
        if (!string.IsNullOrEmpty(model.Description)) existedPaperwork.Description = model.Description;
        if (!string.IsNullOrEmpty(model.IssuedDate)) existedPaperwork.IssuedDate = model.IssuedDate;
        if (model.Price != null) existedPaperwork.Price = model.Price;
        if (!string.IsNullOrEmpty(model.PriceCurrency)) existedPaperwork.PriceCurrency = model.PriceCurrency;
        existedPaperwork.UpdatedBy = _httpContextUtils.GetUserGUID();
        existedPaperwork.UpdatedDate = DateTime.UtcNow.ToString("u");
        var result = await _sqliteDb.UpdateAsync(existedPaperwork);
        if (result <= 0)
        {
            responseData.StatusCode = HttpStatusCode.InternalServerError;
            responseData.Message = $"Something went wrong while updating paperwork: {model.Name}";
            responseData.Data = existedPaperwork;
            return responseData;
        }
        responseData.StatusCode = HttpStatusCode.Created;
        responseData.Message = $"Paperwork {existedPaperwork.Name} update successfully";
        responseData.Data = existedPaperwork;
        return responseData;
    }
    public async Task<GenericResponseData> DeletePaperwork(string categoryGUID, string paperworkGUID)
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
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == selectedFileGUID && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileGUID} not found or deleted";
            return responseData;
        }
        
        // check category is exist or not
        var existedCategory = await _sqliteDb.Table<Categories>().Where(c => c.GUID == categoryGUID && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedCategory == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category {categoryGUID} not found or was deleted";
            return responseData;
        }
        
        // check paperwork is exist or not
        var existedPaperwork = await _sqliteDb.Table<Paperworks>().Where(p => p.GUID == paperworkGUID && p.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedPaperwork == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Paperwork {paperworkGUID} not found or was deleted";
            return responseData;
        }
        
        // Check associated between category and paperwork
        var existedPaperworkCategories = await _sqliteDb.Table<PaperworksCategories>()
            .Where(p => p.PaperworkGUID == paperworkGUID && p.CategoryGUID == categoryGUID && p.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (existedPaperworkCategories == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"There's no associated between paperwork {paperworkGUID} and category {categoryGUID}";
            return responseData;
        }
        
        // update existed paperwork
        existedPaperwork.IsDeleted = 1;
        existedPaperwork.UpdatedBy = _httpContextUtils.GetUserGUID();
        existedPaperwork.UpdatedDate = DateTime.UtcNow.ToString("u");
        var result = await _sqliteDb.UpdateAsync(existedPaperwork);
        if (result <= 0)
        {
            responseData.StatusCode = HttpStatusCode.InternalServerError;
            responseData.Message = $"Something went wrong while deleting paperwork: {existedPaperwork.Name}";
            responseData.Data = existedPaperwork;
            return responseData;
        }
        
        // update associated paperwork and categoroty too
        existedPaperworkCategories.IsDeleted = 1;
        existedPaperworkCategories.UpdatedBy = _httpContextUtils.GetUserGUID();
        existedPaperworkCategories.UpdatedDate = DateTime.UtcNow.ToString("u");
        var result2 = await _sqliteDb.UpdateAsync(existedPaperworkCategories);
        if (result2 <= 0)
        {
            responseData.StatusCode = HttpStatusCode.InternalServerError;
            responseData.Message = $"Something went wrong while deleting paperwork: {existedPaperwork.Name}";
            responseData.Data = existedPaperwork;
            return responseData;
        }

        responseData.StatusCode = HttpStatusCode.OK;
        responseData.Message = $"Paperwork {existedPaperwork.Name} deleted successfully";
        responseData.Data = existedPaperwork;
        return responseData;
    }
}