using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("UsersFiles")]
    public class UsersFiles
    {
        [PrimaryKey, Length(36,36)]
        public string GUID { get; set; }
        [Length(36,36)]
        public string FileGUID { get; set; }
        [Length(36,36)]
        public string UserGUID { get; set; }
        public string Role { get; set; }
        public int? IsSelected { get; set; } = 0;
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("u");
        public int IsDeleted { get; set; } = 0;
    }
}
