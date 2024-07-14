using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;
using mypaperwork.Models;
using mypaperwork.Models.Database;
using mypaperwork.Services.Logging;
using Serilog;
using SQLite;

namespace mypaperwork.Services.Testing;
public class TestingServices
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;
    private readonly LoggingServices _loggingServices;
    private readonly SQLiteAsyncConnection _sqliteDb;
    public TestingServices(IHttpContextAccessor httpContextAccessor, AppSettings appSettings, LoggingServices loggingServices, SQLiteAsyncConnection sqliteDb)
    {
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings;
        _loggingServices = loggingServices;
        _sqliteDb = sqliteDb;
    }

    public async Task<GenericResponseData> GetAppSettings()
    {
        var responseData = new GenericResponseData
        {
            Success = true,
            Data = _appSettings,
            StatusCode = HttpStatusCode.OK,
            Message = "App Settings"
        };

        return responseData;
    }
    public async Task<GenericResponseData> SeriLog()
    {
        Log.Information("Hello world");
        var responseData = new GenericResponseData
        {
            Success = true,
            Data = _appSettings,
            StatusCode = HttpStatusCode.OK,
            Message = "Check the log file"
        };
        return responseData;
    }
    public async Task<GenericResponseData> SeedingDatabase()
    {
        var responseData = new GenericResponseData();
        
        // clean up all tables
        await _sqliteDb.DeleteAllAsync<Users>();
        await _sqliteDb.DeleteAllAsync<FilesDBModel>();
        await _sqliteDb.DeleteAllAsync<PaperWorksCategories>();
        await _sqliteDb.DeleteAllAsync<Categories>();
        await _sqliteDb.DeleteAllAsync<Logs>();
        await _sqliteDb.DeleteAllAsync<UsersFiles>();
        await _sqliteDb.DeleteAllAsync<PaperWorks>();
        
        var user = await _sqliteDb.Table<Users>().Where(u => u.UserName == "tindecken").FirstOrDefaultAsync();
        if (user == null)
        {
        
            // create new user: tindecken/rivaldo
            // Encrypt new password
            MD5 md5 = MD5.Create();
            var tripDes2 = TripleDES.Create();
            tripDes2.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(_appSettings.JWTSecret));
            tripDes2.Mode = CipherMode.ECB;
            tripDes2.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8Encoding.UTF8.GetBytes("rivaldo");
            ICryptoTransform cTransform = tripDes2.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            var encryptedPassword = Convert.ToBase64String(resultArray);
            var newUser = new Users()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Hoang Nguyen",
                Email = "tindecken@gmail.com",
                SystemRole = "SysAdmin",
                UserName = "tindecken",
                Password = encryptedPassword
            };
            await _sqliteDb.InsertAsync(newUser);
        
            var newUser2 = new Users()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Hoang Nguyen 2",
                Email = "tindecken2@gmail.com",
                SystemRole = "SysAdmin",
                UserName = "tindecken2",
                Password = encryptedPassword
            };
            await _sqliteDb.InsertAsync(newUser2);
        
            // create new file
            var fileGUID = "d0286b02-d632-4f4e-a259-5b42c1edff68";
            var file = new Models.Database.FilesDBModel()
            {
                GUID = fileGUID,
                Name = "Tindecken's File",
                Description = "File for user tindecken",
                CreatedBy = newUser.GUID
            };
            await _sqliteDb.InsertAsync(file);
            // Associate user and file with role: ADMIN
            var newUserFile = new UsersFiles()
            {
                GUID = Guid.NewGuid().ToString(),
                UserGUID = newUser.GUID.ToString(),
                FileGUID = fileGUID,
                Role = "Admin",
                IsSelected = 0,
            };
            await _sqliteDb.InsertAsync(newUserFile);
            // Associate user2 and file with role: USER
            var newUserFile2 = new UsersFiles()
            {
                GUID = Guid.NewGuid().ToString(),
                UserGUID = newUser2.GUID.ToString(),
                FileGUID = fileGUID,
                Role = "User",
                IsSelected = 0
            };
            await _sqliteDb.InsertAsync(newUserFile2);
        
            // Create new Category of file 1
            var category = new Categories()
            {
                GUID = Guid.NewGuid().ToString(),
                FileGUID = fileGUID,
                Name = "Hóa Đơn",
                Description = "Hóa Đơn",
                CreatedBy = newUser.GUID
            };
            await _sqliteDb.InsertAsync(category);
        
            // Create new Category 2 of file 1
            var category2 = new Categories()
            {
                GUID = Guid.NewGuid().ToString(),
                FileGUID = fileGUID,
                Name = "Mua sắm",
                Description = "Mua sắm",
                CreatedBy = newUser.GUID
            };
            await _sqliteDb.InsertAsync(category2);
        
            // create paperwork1 for category "Hóa Đơn"
            var paperwork1 = new PaperWorks()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Hóa đơn tháng 1",
                Description = "Hóa đơn tháng 1",
                IssuedDate = "2024-01-01",
                Price = 1000,
                PriceCurrency = "USD",
                CreatedBy = newUser.GUID,
        
            };
            await _sqliteDb.InsertAsync(paperwork1);
        
            // create paperwork 2 for category "Hóa Đơn"
            var paperwork2 = new PaperWorks()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Hóa đơn tháng 2",
                Description = "Hóa đơn tháng 2",
                IssuedDate = "2024-02-01",
                Price = 2000,
                PriceCurrency = "USD",
                CreatedBy = newUser.GUID,
        
            };
            // Associate 2 paperworks with category "Hóa Đơn"
            var paperworkCategory = new PaperWorksCategories()
            {
                GUID = Guid.NewGuid().ToString(),
                PaperWorkGUID = paperwork1.GUID,
                CategoryGUID = category.GUID,
                CreatedBy = newUser.GUID
            };
            await _sqliteDb.InsertAsync(paperworkCategory);
        
            var paperworkCategory2= new PaperWorksCategories()
            {
                GUID = Guid.NewGuid().ToString(),
                PaperWorkGUID = paperwork2.GUID,
                CategoryGUID = category.GUID,
                CreatedBy = newUser.GUID
            };
            await _sqliteDb.InsertAsync(paperworkCategory2);
            await _sqliteDb.InsertAsync(paperwork2);
            
            // Create 1500 paperworks with catgory "Hóa Đơn"
            for (int i = 1; i <= 1500; i++)
            {
                var newPaperwork = new PaperWorks()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Name = $"Hóa đơn {i}",
                    Description = $"Hóa đơn {i}",
                    IssuedDate = DateTime.UtcNow.AddDays(-i).ToString("u"),
                    Price = (i+1000),
                    PriceCurrency = "USD",
                    CreatedBy = newUser.GUID,
        
                };
                await _sqliteDb.InsertAsync(newPaperwork);
                // associate newPaperwork with category "Hóa Đơn"
                var newPaperworkCategory = new PaperWorksCategories()
                {
                    GUID = Guid.NewGuid().ToString(),
                    PaperWorkGUID = newPaperwork.GUID,
                    CategoryGUID = category.GUID,
                    CreatedBy = newUser.GUID
                };
                await _sqliteDb.InsertAsync(newPaperworkCategory);
            }
            // create 2 paperworks and associate with category "Mua Sắm"
            var paperwork3 = new PaperWorks()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Mua Sắm 1",
                Description = "Mua sắm 1",
                IssuedDate = "2024-01-01",
                Price = 1500,
                PriceCurrency = "USD",
                CreatedBy = newUser.GUID,
        
            };
            await _sqliteDb.InsertAsync(paperwork3);
        
            // create paperwork 4 for category "Mua Sắm"
            var paperwork4 = new PaperWorks()
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Mua Sắm 2",
                Description = "Mua sắm 2",
                IssuedDate = "2024-02-01",
                Price = 2500,
                PriceCurrency = "USD",
                CreatedBy = newUser.GUID,
        
            };
            await _sqliteDb.InsertAsync(paperwork4);
        
            // Associate 2 paperworks with category "Mua Sắm"
            var paperworkCategory3 = new PaperWorksCategories()
            {
                GUID = Guid.NewGuid().ToString(),
                PaperWorkGUID = paperwork3.GUID,
                CategoryGUID = category2.GUID,
                CreatedBy = newUser.GUID
            };
            await _sqliteDb.InsertAsync(paperworkCategory3);    
        
            var paperworkCategory4 = new PaperWorksCategories()
            {
                GUID = Guid.NewGuid().ToString(),
                PaperWorkGUID = paperwork4.GUID,
                CategoryGUID = category2.GUID,
                CreatedBy = newUser.GUID
            };
            await _sqliteDb.InsertAsync(paperworkCategory4);    
            
            // Create 1500 paperworks with catgory "Mua Sắm"
            for (int i = 1; i <= 1500; i++)
            {
                var newPaperwork = new PaperWorks()
                {
                    GUID = Guid.NewGuid().ToString(),
                    Name = $"Mua Sắm {i}",
                    Description = $"Mua Sắm {i}",
                    IssuedDate = DateTime.UtcNow.AddDays(-i).ToString("u"),
                    Price = (i+5000),
                    PriceCurrency = "USD",
                    CreatedBy = newUser.GUID,
        
                };
                await _sqliteDb.InsertAsync(newPaperwork);
                // associate newPaperwork with category "Mua Sắm"
                var newPaperworkCategory = new PaperWorksCategories()
                {
                    GUID = Guid.NewGuid().ToString(),
                    PaperWorkGUID = newPaperwork.GUID,
                    CategoryGUID = category2.GUID,
                    CreatedBy = newUser.GUID
                };
                await _sqliteDb.InsertAsync(newPaperworkCategory);
            }
        
            responseData.Data = newUser;
            responseData.Success = true;
            responseData.Message = "Successfully seeding database !";
            return responseData;
        }

        responseData.Data = null;
        responseData.Success = true;
        responseData.Message = "Successfully seeding database !";
        return responseData;
    }
}