using SQLite;

namespace mypaperwork.Models.Database
{
    [Table("Users")]
    public class Users
    {
        [PrimaryKey]
        public string GUID { get; set; }
        public string Name { get; set; }
        [Unique]
        public string UserName { get; set; }
        [Unique]
        public string Email { get; set; }
        public string Password { get; set; }
        public string SystemRole { get; set; }
        public int IsDeleted { get; set; } = 1;
    }
}
