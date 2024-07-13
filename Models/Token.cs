using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models
{
    public class Token
    {
        [Length(36,36)]
        public string userGUID { get; set; }
        public string systemRole { get; set; }
        public string email { get; set; }
        public string selectedFileGUID { get; set; }
        public string selectedFileRole { get; set; }
    }
}
