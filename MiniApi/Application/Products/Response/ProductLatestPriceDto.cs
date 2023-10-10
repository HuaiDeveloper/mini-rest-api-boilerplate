namespace MiniApi.Application.Products.Response;

public class ProductLatestPriceDto
{
    public long ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal? LatestPrice { get; set; }
    public DateTime? CurrentDate { get; set; }
}