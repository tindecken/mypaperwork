using System.ComponentModel.DataAnnotations;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("UsersSettings")]
    public class UsersSettings
    {
        [PrimaryKey, Length(26,26)]
        public string Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string SettingId { get; set; }
        public string Descritpion { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("u");
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
}
