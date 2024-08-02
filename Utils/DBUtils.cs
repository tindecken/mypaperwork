using mypaperwork.Services.Logging;
using SQLite;
using System.Net.Mail;
using mypaperwork.Models.Database;

namespace mypaperwork.Utils;

public class DBUtils
{
    private readonly SQLiteAsyncConnection _sqliteDb;
    public DBUtils(SQLiteAsyncConnection sqliteDb)
    {
        _sqliteDb = sqliteDb;
    }
    public async Task SetSelectedFileAsync(string userId, string fileId)
    {
        var usersFiles = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserId == userId && uf.IsDeleted == 0).ToListAsync();
        if (usersFiles.Count > 0)
        {
            foreach (var user in usersFiles)
            {
                user.IsSelected = 0;
                await _sqliteDb.UpdateAsync(user);
            }   
            var userFile = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserId == userId && uf.FileId == fileId).FirstOrDefaultAsync();
            if (userFile != null)
            {
                userFile.IsSelected = 1;
                await _sqliteDb.UpdateAsync(userFile);
            }
        }
        
    }
    public async Task<string?> GetSelectedUsersFilesId(string userId, string fileId)
    {
        var selectedUserFile = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserId == userId && uf.IsDeleted == 0 && uf.FileId == fileId).FirstOrDefaultAsync();
        if (selectedUserFile != null)
        {
            return selectedUserFile.FileId;
        }
        else return null;
    }
}