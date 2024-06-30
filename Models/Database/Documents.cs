using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Documents")]
    public class Documents
    {
        [PrimaryKey]
        public string GUID { get; set; }
        public string PaperWorkGUID { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public byte[] FileBlob { get; set; }
        public string CreatedDate { get; set; } = DateTime.Now.ToString("u");
        public string? CreatedBy { get; set; }
    }
}
