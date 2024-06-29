using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Logs")]
    public class Logs
    {
        [PrimaryKey]
        public string UUID { get; set; }
        public string? ActionType { get; set; }
        public string? Method { get; set; }
        public string? Message { get; set; }
        public string? OldData { get; set; }
        public string? NewData { get; set; }
        public int? ActionById { get; set; }
        public string? IPAddress { get; set; }
        public string ActionDate { get; set; } = DateTime.UtcNow.ToString("u");
    }
}
