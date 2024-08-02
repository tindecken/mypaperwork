using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Paperwork;

public class CreatePaperworkRequestModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required, Length(26, 26)]
    public string CategoryId { get; set; }
    [MaxLength(5000)]
    public string? Description { get; set; }
    public string? IssuedDate { get; set; }
    public decimal? Price { get; set; }
    public string? PriceCurrency { get; set; }
    
}