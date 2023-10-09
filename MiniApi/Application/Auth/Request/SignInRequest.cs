namespace MiniApi.Application.Auth.Request;

public class SignInRequest
{
    public string Name { get; set; } = default!;
    public string Password { get; set; } = default!;
}