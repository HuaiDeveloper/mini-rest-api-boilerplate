namespace MiniApi.Application.Products.Response;

public class ProductDetailDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
    public List<CurrentPriceDetailDto>? CurrentPrices { get; set; }
}
