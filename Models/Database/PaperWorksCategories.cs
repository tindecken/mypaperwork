using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("PaperWorksCategories")]
    public class PaperWorksCategories
    {
        [PrimaryKey]
        public string GUID { get; set; }
        public string PaperWorkGUID { get; set; }
        public string? CategoryGUID { get; set; }
        public string CreatedDate { get; set; } = DateTime.Now.ToString("u");
        public string? CreatedBy { get; set; }
        public int IsDeleted { get; set; } = 0;

    }
}
