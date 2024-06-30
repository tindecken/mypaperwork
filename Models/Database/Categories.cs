using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Categories")]
    public class Categories
    {
        [PrimaryKey]
        public string GUID { get; set; }
        public string FileGUID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CreatedDate { get; set; } = DateTime.Now.ToString("u");
        public string? CreatedBy { get; set; }
        public string? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
