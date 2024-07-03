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
    public async Task SetSelectedFileAsync(string userGUID, string fileGUID)
    {
        var users = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.UserGUID == userGUID && uf.IsDeleted == 0).ToListAsync();
        if (users.Count > 0)
        {
            foreach (var user in users)
            {
                user.IsSelected = 0;
                await _sqliteDb.UpdateAsync(user);
            }   
            var userFile = await _sqliteDb.Table<UsersFiles>().Where(uf => uf.FileGUID == fileGUID).FirstOrDefaultAsync();
            if (userFile != null)
            {
                userFile.IsSelected = 1;
                await _sqliteDb.UpdateAsync(userFile);
            }
        }
        
    }
}