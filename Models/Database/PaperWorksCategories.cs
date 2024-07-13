using System.ComponentModel.DataAnnotations;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("PaperWorksCategories")]
    public class PaperWorksCategories
    {
        [PrimaryKey, Length(36,36)]
        public string GUID { get; set; }
        [Length(36,36)]
        public string PaperWorkGUID { get; set; }
        [Length(36,36)]
        public string? CategoryGUID { get; set; }
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("u");
        public string? CreatedBy { get; set; }
        public int IsDeleted { get; set; } = 0;

    }
}
