using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Categories
{
    public class UpdateCategoryRequestModel
    {
        [Required, Length(26,26)]
        public string Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
