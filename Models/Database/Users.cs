using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Users")]
    public class Users
    {
        [PrimaryKey]
        public string GUID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
