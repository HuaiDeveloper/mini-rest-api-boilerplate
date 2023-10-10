namespace MiniApi.Model;

public class CurrentPrice
{
    public long Id { get; private set; }
    public long ProductId { get; private set; }
    public virtual Product? Product { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CurrentDate { get; private set; }
    public string? Description { get; private set; }

    public CurrentPrice(long productId, decimal price, DateTime currentDate, string? description)
    {
        ProductId = productId;
        Price = price;
        CurrentDate = currentDate;
        Description = description;
    }

    public CurrentPrice Update(decimal price, DateTime currentDate, string? description)
    {
        Price = price;
        CurrentDate = currentDate;
        Description = description;

        return this;
    }
}