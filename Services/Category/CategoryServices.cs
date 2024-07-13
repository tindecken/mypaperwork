using System.Net;
using mypaperwork.Models;
using mypaperwork.Models.Categories;
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

    public CategoryServices(HttpContextUtils httpContextUtils, AppSettings appSettings, LoggingServices loggingServices, SQLiteAsyncConnection sqliteDb, DBUtils dbUtils)
    {
        _httpContextUtils = httpContextUtils;
        _appSettings = appSettings;
        _loggingServices = loggingServices;
        _sqliteDb = sqliteDb;
        _dbUtils = dbUtils;
    }

    public async Task<GenericResponseData> CreateNewCategory(CreateCategoryRequestModel cat)
    {
        var responseData = new GenericResponseData();
        var selectedFileGUID = _httpContextUtils.GetSelectedFileGUID();
        if (string.IsNullOrEmpty(selectedFileGUID))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check file is exist
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.GUID == selectedFileGUID && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"File {selectedFileGUID} not found";
            return responseData;
        }
        var existingCategory = await _sqliteDb.Table<Categories>().Where(c => c.Name == cat.Name && c.FileGUID == selectedFileGUID && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existingCategory != null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category Name {cat.Name} is duplicated.";
            return responseData;
        }

        var category = new Categories()
        {
            GUID = Guid.NewGuid().ToString(),
            FileGUID = selectedFileGUID,
            Name = cat.Name,
            Description = cat.Description,
            CreatedBy = _httpContextUtils.GetUserGUID(),
        };
        await _sqliteDb.InsertAsync(category);  
        responseData.Data = category;
        responseData.Message = $"Category {cat.Name} created successfully";
        return responseData;
    }
    public async Task<GenericResponseData> UpdateCategory(UpdateCategoryRequestModel cat)
    {
        var responseData = new GenericResponseData();
        var selectedFileGUID = _httpContextUtils.GetSelectedFileGUID();
        if (string.IsNullOrEmpty(selectedFileGUID))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }

        // check file is exist
        var file = await _sqliteDb.Table<Categories>().Where(c => c.FileGUID == selectedFileGUID).FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"File {selectedFileGUID} not found";
            return responseData;
        }
        var category = await _sqliteDb.Table<Categories>().Where(c => c.GUID == cat.GUID && c.FileGUID == selectedFileGUID && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (category == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"Category {cat.GUID} not found";
            return responseData;
        }
        // check existing category by Name
        var existingCategoryName = await _sqliteDb.Table<Categories>().Where(c => c.Name == cat.Name && c.FileGUID == selectedFileGUID && c.IsDeleted == 0 && c.GUID != cat.GUID).FirstOrDefaultAsync();
        if (existingCategoryName != null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category Name {cat.Name} is duplicated.";
            return responseData;
        }   
        category.Name = cat.Name;
        category.Description = cat.Description;
        category.UpdatedBy = _httpContextUtils.GetUserGUID();
        category.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(category);
        responseData.Data = category;
        responseData.Message = $"Category {cat.Name} updated successfully";
        return responseData;
    }
    public async Task<GenericResponseData> DeleteCategory(string categoryGUID)
    {
        var responseData = new GenericResponseData();
        var selectedFileGUID = _httpContextUtils.GetSelectedFileGUID();
        if (string.IsNullOrEmpty(selectedFileGUID))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check category is existed along with file
        var category = await _sqliteDb.Table<Categories>().Where(c => c.GUID == categoryGUID && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (category == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"Category {categoryGUID} is not found or was deleted";
            return responseData;
        }
        // check category is existed along with file
        var categoryWithFile = await _sqliteDb.Table<Categories>().Where(c => c.GUID == categoryGUID && c.FileGUID == selectedFileGUID && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (categoryWithFile == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"Category {categoryGUID} with selected file {selectedFileGUID} not found";
            return responseData;
        }
        // check existing category by Name
        category.IsDeleted = 1;
        category.UpdatedBy = _httpContextUtils.GetUserGUID();
        category.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(category);
        responseData.Data = category;
        responseData.Message = $"Deleted category {category.Name} successfully";
        return responseData;
    }
}