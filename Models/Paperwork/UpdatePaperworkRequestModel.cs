using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Paperwork;

public class UpdatePaperworkRequestModel
{
    [Required, Length(36,36)]
    public string GUID { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }
    [MaxLength(5000)]
    public string? Description { get; set; }
    public string? IssuedDate { get; set; }
    public decimal? Price { get; set; }
    public string? PriceCurrency { get; set; }
}