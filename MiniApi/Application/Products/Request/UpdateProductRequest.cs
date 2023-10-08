namespace MiniApi.Application.Products.Request;

public class UpdateProductRequest
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
