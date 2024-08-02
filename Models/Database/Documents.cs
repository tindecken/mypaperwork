using System.ComponentModel.DataAnnotations;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Documents")]
    public class Documents
    {
        [PrimaryKey, Length(26,26)]
        public string Id { get; set; }
        [Length(26,26)]
        public string PaperWorkId { get; set; }
        public string Folder { get; set; }
        public string FileName { get; set; }
        public decimal FileSize { get; set; }
        public string CreatedDate { get; set; } = DateTime.UtcNow.ToString("u");
        public string? CreatedBy { get; set; }
        public int IsDeleted { get; set; } = 0;
    }
}
