namespace MiniApi.Model;

public class SerialNumberCode
{
    public long Id { get; private set; }
    public string NumberCode { get; private set; }
    public DateTime GenerateOn { get; private set; }
    public string? Description { get; private set; }

    public SerialNumberCode(long id, string numberCode, string? description)
    {
        Id = id;
        NumberCode = numberCode;
        GenerateOn = DateTime.UtcNow;
        Description = description;
    }

    public SerialNumberCode Update(string? description)
    {
        Description = description;

        return this;
    }
}