using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Files;

public class CreateFileRequestModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(5000)]
    public string? Description { get; set; }
}