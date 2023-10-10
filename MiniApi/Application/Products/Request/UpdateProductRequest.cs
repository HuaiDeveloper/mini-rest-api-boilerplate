using System.ComponentModel.DataAnnotations;

namespace MiniApi.Application.Products.Request;

public class UpdateProductRequest
{
    [Required]
    public long Id { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = default!;
    
    [MaxLength(50)]
    public string? Description { get; set; }
}
