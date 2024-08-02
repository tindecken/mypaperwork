using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models
{
    public class Token
    {
        [Length(26,26)]
        public string userId { get; set; }
        public string systemRole { get; set; }
        public string email { get; set; }
        public string selectedFileId { get; set; }
        public string selectedFileRole { get; set; }
    }
}
