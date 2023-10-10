namespace MiniApi.Application.Products.Response;

public class CurrentPriceDetailDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public decimal Price { get; set; }
    public DateTime CurrentDate { get; set; }
    public string? Description { get; set; }
}