namespace MiniApi.Applicatoin.Products.Request
{
    public class UpdateProductRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
