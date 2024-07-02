namespace mypaperwork
{
    public class AppSettings
    {
        public string JWTSecret { get; set; }
        public string SQLiteDBSecret { get; set; }
        public string SQLiteDBPath { get; set; }
        public string StoragePath { get; set; }
        public decimal MaxFileSize { get; set; }
    }
}
