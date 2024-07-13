using System.ComponentModel.DataAnnotations;

namespace mypaperwork.Models.Paperwork;

public class CreatePaperworkRequestModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Length(36, 36)]
    public string CategoryGUID { get; set; }
    [MaxLength(1000)]
    public string Description { get; set; }
    public string IssuedDate { get; set; }
    public decimal Price { get; set; }
    public string PriceCurrency { get; set; }
    
}