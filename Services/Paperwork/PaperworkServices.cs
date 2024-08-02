using System.Net;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Models.Filter;
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
        var selectedFileId = _httpContextUtils.GetSelectedFileId();
        if (string.IsNullOrEmpty(selectedFileId))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check file is existed or not
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileId} not found or deleted";
            return responseData;
        }
        // check category (with file) is exist or not
        var existedCategory = await _sqliteDb.Table<Categories>().Where(c => c.Id == model.CategoryId && c.FileId == selectedFileId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedCategory == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category {model.CategoryId} associated with file {selectedFileId} not found or was deleted";
            return responseData;
        }
        // create new paperwork
        var paperwork = new Paperworks()
        {
            Id = Ulid.NewUlid().ToString(),
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            IssuedDate = model.IssuedDate,
            Price = model.Price,
            PriceCurrency = model.PriceCurrency,
            CreatedBy = _httpContextUtils.GetUserId(),
        };
        var result = await _sqliteDb.InsertAsync(paperwork);
        if (result <= 0)
        {
            responseData.StatusCode = HttpStatusCode.InternalServerError;
            responseData.Message = $"Something went wrong while creating new paperwork: {model.Name.Trim()}";
            responseData.Data = paperwork;
            return responseData;
        }
        // create associate between category and paperwork
        var categoryPaperwork = new PaperworksCategories()
        {
            Id = Ulid.NewUlid().ToString(),
            CategoryId = model.CategoryId,
            PaperworkId = paperwork.Id,
            CreatedBy = _httpContextUtils.GetUserId(),
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
        var selectedFileId = _httpContextUtils.GetSelectedFileId();
        if (string.IsNullOrEmpty(selectedFileId))
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = "Please select a file first";
            return responseData;
        }
        // check file is existed or not
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileId} not found or deleted";
            return responseData;
        }
        
        // check paperwork is exist or not
        var existedPaperwork = await _sqliteDb.Table<Paperworks>().Where(p => p.Id == model.Id && p.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedPaperwork == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Paperwork {model.Id} not found or was deleted";
            return responseData;
        }
        
        // update existed paperwork
        if (!string.IsNullOrEmpty(model.Name)) existedPaperwork.Name = model.Name.Trim();
        if (!string.IsNullOrEmpty(model.Description)) existedPaperwork.Description = model.Description.Trim();
        if (!string.IsNullOrEmpty(model.IssuedDate)) existedPaperwork.IssuedDate = model.IssuedDate;
        if (model.Price != null) existedPaperwork.Price = model.Price;
        if (!string.IsNullOrEmpty(model.PriceCurrency)) existedPaperwork.PriceCurrency = model.PriceCurrency;
        existedPaperwork.UpdatedBy = _httpContextUtils.GetUserId();
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
    public async Task<GenericResponseData> DeletePaperwork(string categoryId, string paperworkId)
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
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileId} not found or deleted";
            return responseData;
        }
        
        // check category is exist or not
        var existedCategory = await _sqliteDb.Table<Categories>().Where(c => c.Id == categoryId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedCategory == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category {categoryId} not found or was deleted";
            return responseData;
        }
        
        // check paperwork is exist or not
        var existedPaperwork = await _sqliteDb.Table<Paperworks>().Where(p => p.Id == paperworkId && p.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedPaperwork == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Paperwork {paperworkId} not found or was deleted";
            return responseData;
        }
        
        // Check associated between category and paperwork
        var existedPaperworkCategories = await _sqliteDb.Table<PaperworksCategories>()
            .Where(p => p.PaperworkId == paperworkId && p.CategoryId == categoryId && p.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (existedPaperworkCategories == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"There's no associated between paperwork {paperworkId} and category {categoryId}";
            return responseData;
        }
        
        // update existed paperwork
        existedPaperwork.IsDeleted = 1;
        existedPaperwork.UpdatedBy = _httpContextUtils.GetUserId();
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
        existedPaperworkCategories.UpdatedBy = _httpContextUtils.GetUserId();
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
    public async Task<GenericResponseData> GetByFile(PaginationFilter filter)
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
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileId} not found or deleted";
            return responseData;
        }
        
        // get all categories associate with selected file
        var categories = await _sqliteDb.Table<Categories>().Where(c => c.FileId == selectedFileId && c.IsDeleted == 0).OrderByDescending(x => x.CreatedDate).ToListAsync();

        // get all paperworksCategories associated
        var paperworksCategories = new List<PaperworksCategories>();
        foreach (var category in categories)
        {
            var papersCats = await _sqliteDb.Table<PaperworksCategories>().Where(p => p.CategoryId == category.Id && p.IsDeleted == 0).ToListAsync();
            foreach (var pc in papersCats)
            {
                paperworksCategories.Add(pc);
            }
        }

        var paperworks = new List<Paperworks>();
        foreach (var pc in paperworksCategories)
        {
            var p = await _sqliteDb.Table<Paperworks>().Where(x => x.Id == pc.PaperworkId && x.IsDeleted == 0).FirstOrDefaultAsync();
            if (p !=null ) paperworks.Add(p);
        }
        
        var paperworksQuery = paperworks.AsQueryable();
        
        // Filtering
        if (!string.IsNullOrEmpty(filter.FilterValue))
        {
            var filterValue = filter.FilterValue.ToLower();
            paperworksQuery = paperworksQuery.Where(x =>
                x.Id.ToLower().Contains(filterValue) ||
                x.CreatedDate.ToString().ToLower().Contains(filterValue) ||
                x.Name.ToString().ToLower().Contains(filterValue) ||
                x.Description.ToString().ToLower().Contains(filterValue) ||
                x.Price.ToString().ToLower().Contains(filterValue) ||
                x.PriceCurrency.ToLower().Contains(filterValue) ||
                x.IssuedDate.ToLower().Contains(filterValue));
        }
        // Sorting
        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            switch (filter.SortBy.ToLower())
            {
                case "name":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.Name) : paperworksQuery.OrderBy(x => x.Name);
                    break;
                case "description":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.Description) : paperworksQuery.OrderBy(x => x.Description);
                    break;
                case "createddate":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.CreatedDate) : paperworksQuery.OrderBy(x => x.CreatedDate);
                    break;
                case "price":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.Price) : paperworksQuery.OrderBy(x => x.Price);
                    break;
                case "pricecurrency":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.PriceCurrency) : paperworksQuery.OrderBy(x => x.PriceCurrency);
                    break;
                case "issueddate":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.IssuedDate) : paperworksQuery.OrderBy(x => x.IssuedDate);
                    break;
                default:
                    responseData.Success = false;
                    responseData.StatusCode = HttpStatusCode.BadRequest;
                    responseData.Message = "Invalid input sort by.";
                    return responseData;
            }
        }
        
        paperworksQuery = paperworksQuery.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        var totalRecords = paperworks.Count;

        responseData.Data = paperworksQuery;
        responseData.Message = $"Retrieved {paperworksQuery.Count()}/{totalRecords} record(s) successfully.";
        responseData.Success = true;
        responseData.TotalRecords = totalRecords;
        responseData.TotalFilteredRecords = paperworksQuery.Count();
        responseData.PageNumber = filter.PageNumber;
        responseData.PageSize = filter.PageSize;
        return responseData;
    }
    public async Task<GenericResponseData> GetByCategory(string categoryId, PaginationFilter filter)
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
        var file = await _sqliteDb.Table<FilesDBModel>().Where(f => f.Id == selectedFileId && f.IsDeleted == 0)
            .FirstOrDefaultAsync();
        if (file == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"File {selectedFileId} not found or deleted";
            return responseData;
        }
        
        var existedCategory = await _sqliteDb.Table<Categories>().Where(c => c.Id == categoryId && c.FileId == selectedFileId && c.IsDeleted == 0).FirstOrDefaultAsync();
        if (existedCategory == null)
        {
            responseData.StatusCode = HttpStatusCode.BadRequest;
            responseData.Message = $"Category {categoryId} not found or was deleted";
            return responseData;
        }

        // get all paperworksCategories associated
        var paperworksCategories = new List<PaperworksCategories>();
        var papersCats = await _sqliteDb.Table<PaperworksCategories>().Where(p => p.CategoryId == existedCategory.Id && p.IsDeleted == 0).ToListAsync();
        foreach (var pc in papersCats)
        {
            paperworksCategories.Add(pc);
        }

        var paperworks = new List<Paperworks>();
        foreach (var pc in paperworksCategories)
        {
            var p = await _sqliteDb.Table<Paperworks>().Where(x => x.Id == pc.PaperworkId && x.IsDeleted == 0).FirstOrDefaultAsync();
            if (p !=null ) paperworks.Add(p);
        }
        
        var paperworksQuery = paperworks.AsQueryable();
        
        // Filtering
        if (!string.IsNullOrEmpty(filter.FilterValue))
        {
            var filterValue = filter.FilterValue.ToLower();
            paperworksQuery = paperworksQuery.Where(x =>
                x.Id.ToLower().Contains(filterValue) ||
                x.CreatedDate.ToString().ToLower().Contains(filterValue) ||
                x.Name.ToString().ToLower().Contains(filterValue) ||
                x.Description.ToString().ToLower().Contains(filterValue) ||
                x.Price.ToString().ToLower().Contains(filterValue) ||
                x.PriceCurrency.ToLower().Contains(filterValue) ||
                x.IssuedDate.ToLower().Contains(filterValue));
        }
        // Sorting
        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            switch (filter.SortBy.ToLower())
            {
                case "name":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.Name) : paperworksQuery.OrderBy(x => x.Name);
                    break;
                case "description":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.Description) : paperworksQuery.OrderBy(x => x.Description);
                    break;
                case "createddate":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.CreatedDate) : paperworksQuery.OrderBy(x => x.CreatedDate);
                    break;
                case "price":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.Price) : paperworksQuery.OrderBy(x => x.Price);
                    break;
                case "pricecurrency":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.PriceCurrency) : paperworksQuery.OrderBy(x => x.PriceCurrency);
                    break;
                case "issueddate":
                    paperworksQuery = (bool)filter.IsSortDescending ? paperworksQuery.OrderByDescending(x => x.IssuedDate) : paperworksQuery.OrderBy(x => x.IssuedDate);
                    break;
                default:
                    responseData.Success = false;
                    responseData.StatusCode = HttpStatusCode.BadRequest;
                    responseData.Message = "Invalid input sort by.";
                    return responseData;
            }
        }
        
        paperworksQuery = paperworksQuery.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        var totalRecords = paperworks.Count;

        responseData.Data = paperworksQuery;
        responseData.Message = $"Retrieved {paperworksQuery.Count()}/{totalRecords} record(s) successfully.";
        responseData.Success = true;
        responseData.TotalRecords = totalRecords;
        responseData.TotalFilteredRecords = paperworksQuery.Count();
        responseData.PageNumber = filter.PageNumber;
        responseData.PageSize = filter.PageSize;
        return responseData;
    }
}