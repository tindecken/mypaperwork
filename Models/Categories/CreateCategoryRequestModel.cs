using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Categories
{
    public class CreateCategoryRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Build { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CreatedById { get; set; }
        public bool IsOfficial { get; set; }
        public List<RegressionTestDTO> RegressionTests { get; set; }
    }
}
