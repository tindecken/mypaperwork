using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("UsersFiles")]
    public class UsersFiles
    {
        [PrimaryKey]
        public string GUID { get; set; }
        public string FileGUID { get; set; }
        public string UserGUID { get; set; }
        public string Role { get; set; }
    }
}
