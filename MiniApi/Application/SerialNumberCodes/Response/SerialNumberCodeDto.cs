namespace MiniApi.Application.SerialNumberCodes.Response;

public class SerialNumberCodeDto
{
    public long Id { get; set; }
    public string NumberCode { get; set; } = default!;
    public DateTime GenerateOn { get; set; }
}