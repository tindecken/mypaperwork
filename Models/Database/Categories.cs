using System.ComponentModel.DataAnnotations;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Categories")]
    public class Categories
    {
        [PrimaryKey, Length(36,36)]
        public string GUID { get; set; }
        [NotNull, Length(36,36)]
        public string FileGUID { get; set; }
        [NotNull]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("u");
        public string? CreatedBy { get; set; }
        public string? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
}
