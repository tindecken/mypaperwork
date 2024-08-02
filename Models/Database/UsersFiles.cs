using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("UsersFiles")]
    public class UsersFiles
    {
        [PrimaryKey, Length(26,26)]
        public string Id { get; set; }
        [Length(26,26)]
        public string FileId { get; set; }
        [Length(26,26)]
        public string UserId { get; set; }
        public string Role { get; set; }
        public int? IsSelected { get; set; } = 0;
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("u");
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
}
