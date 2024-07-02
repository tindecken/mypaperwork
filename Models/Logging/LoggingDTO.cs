namespace mypaperwork.Models.Logging
{
    public class LoggingDTO
    {
        public int Id { get; set; }
        public string? ActionType { get; set; }
        public string? Method { get; set; }
        public string? Message { get; set; }
        public string? OldData { get; set; }
        public string? NewData { get; set; }
        public string? ActionBy { get; set; }
        public string? IPAddress { get; set; }
        public DateTime? ActionDate { get; set; }
    }
    public enum ActionType
    {
        Create,
        Update,
        Delete,
        LoginSuccess,
        LoginFailed,
        ChangePassword
    }
}
