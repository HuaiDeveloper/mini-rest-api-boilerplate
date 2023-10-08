namespace MiniApi.Application.Products.Request
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
