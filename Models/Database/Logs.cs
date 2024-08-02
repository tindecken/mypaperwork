using System.ComponentModel.DataAnnotations;
using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Logs")]
    public class Logs
    {
        [PrimaryKey, Length(26,26)]
        public string Id { get; set; }
        public string? ActionType { get; set; }
        public string? Method { get; set; }
        public string? Message { get; set; }
        public string? OldData { get; set; }
        public string? NewData { get; set; }
        public string? ActionBy { get; set; }
        public string? IPAddress { get; set; }
        public string CreateDate { get; set; } = DateTime.UtcNow.ToString("u");
    }
}
