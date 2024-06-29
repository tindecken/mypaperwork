using SQLite;
using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Database
{
    [Table("Users")]
    public class Users
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MinLength(3), SQLite.MaxLength(150), Required]
        public string Name { get; set; }
        [MinLength(3), SQLite.MaxLength(150), Required]
        public string UserName { get; set; }
        [MinLength(3), SQLite.MaxLength(150), Required]
        public string Email { get; set; }
        [MinLength(3), Required]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
