using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Categories
{
    public class UpdateCategoryRequestModel
    {
        [Required]
        public string GUID { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
