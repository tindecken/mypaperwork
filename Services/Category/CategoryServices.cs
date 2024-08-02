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
        var selectedFileId = _httpContextUtils.GetSelectedFileId();
        if (string.IsNullOrEmpty(selectedFileId))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check file is exist
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"File {selectedFileId} not found";
            return responseData;
        }
        var existingCategory = await _sqliteDb.Table<Categories>().Where(c => c.Name == cat.Name && c.FileId == selectedFileId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existingCategory != null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category Name {cat.Name} is duplicated.";
            return responseData;
        }

        var category = new Categories()
        {
            Id = Ulid.NewUlid().ToString(),
            FileId = selectedFileId,
            Name = cat.Name.Trim(),
            Description = cat.Description?.Trim(),
            CreatedBy = _httpContextUtils.GetUserId(),
        };
        await _sqliteDb.InsertAsync(category);  
        responseData.Data = category;
        responseData.Message = $"Category {cat.Name.Trim()} created successfully";
        return responseData;
    }
    public async Task<GenericResponseData> UpdateCategory(UpdateCategoryRequestModel cat)
    {
        var responseData = new GenericResponseData();
        var selectedFileId = _httpContextUtils.GetSelectedFileId();
        if (string.IsNullOrEmpty(selectedFileId))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }

        // check file is exist
        var file = await _sqliteDb.Table<Categories>().Where(c => c.FileId == selectedFileId).FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"File {selectedFileId} not found";
            return responseData;
        }
        var category = await _sqliteDb.Table<Categories>().Where(c => c.Id == cat.Id && c.FileId == selectedFileId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (category == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"Category {cat.Id} not found";
            return responseData;
        }
        // check existing category by Name
        var existingCategoryName = await _sqliteDb.Table<Categories>().Where(c => c.Name == cat.Name.Trim() && c.FileId == selectedFileId && c.IsDeleted == 0 && c.Id != cat.Id).FirstOrDefaultAsync();
        if (existingCategoryName != null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category Name {cat.Name.Trim()} is duplicated.";
            return responseData;
        }   
        category.Name = cat.Name.Trim();
        category.Description = cat.Description;
        category.UpdatedBy = _httpContextUtils.GetUserId();
        category.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(category);
        responseData.Data = category;
        responseData.Message = $"Category {cat.Name.Trim()} updated successfully";
        return responseData;
    }
    public async Task<GenericResponseData> DeleteCategory(string categoryId)
    {
        var responseData = new GenericResponseData();
        var selectedFileId = _httpContextUtils.GetSelectedFileId();
        if (string.IsNullOrEmpty(selectedFileId))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check category is existed along with file
        var category = await _sqliteDb.Table<Categories>().Where(c => c.Id == categoryId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (category == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"Category {categoryId} is not found or was deleted";
            return responseData;
        }
        // check category is existed along with file
        var categoryWithFile = await _sqliteDb.Table<Categories>().Where(c => c.Id == categoryId && c.FileId == selectedFileId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (categoryWithFile == null)
        {
            responseData.StatusCode = HttpStatusCode.NotFound;
            responseData.Message = $"Category {categoryId} with selected file {selectedFileId} not found";
            return responseData;
        }
        // check existing category by Name
        category.IsDeleted = 1;
        category.UpdatedBy = _httpContextUtils.GetUserId();
        category.UpdatedDate = DateTime.UtcNow.ToString("u");
        await _sqliteDb.UpdateAsync(category);
        responseData.Data = category;
        responseData.Message = $"Deleted category {category.Name} successfully";
        return responseData;
    }
}