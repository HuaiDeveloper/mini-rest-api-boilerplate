namespace MiniApi.Application.Auth.Request;

public class SignUpRequest
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string? Description { get; set; }
}