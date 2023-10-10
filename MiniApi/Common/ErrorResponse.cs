namespace MiniApi.Common;

public class ErrorResponse
{
    public string Message { get; set; } = default!;
    public List<string>? ErrorMessages { get; set; }
}