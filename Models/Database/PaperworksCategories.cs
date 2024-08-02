using System.ComponentModel.DataAnnotations;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("PaperworksCategories")]
    public class PaperworksCategories
    {
        [PrimaryKey, Length(26,26)]
        public string Id { get; set; }
        [Length(26,26)]
        public string PaperworkId { get; set; }
        [Length(26,26)]
        public string? CategoryId { get; set; }
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("u");
        public string? CreatedBy { get; set; }
        public string? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public int IsDeleted { get; set; } = 0;

    }
}
