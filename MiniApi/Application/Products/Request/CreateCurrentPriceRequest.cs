using System.ComponentModel.DataAnnotations;

namespace MiniApi.Application.Products.Request;

public class CreateCurrentPriceRequest
{
    [Required]
    public long ProductId { get; set; }
    
    [Required]
    [Range(0.0, Double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    public DateTime CurrentDate { get; set; }
    
    [MaxLength(20)]
    public string? Description { get; set; }
}