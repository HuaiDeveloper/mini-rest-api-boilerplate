using System.ComponentModel.DataAnnotations;

namespace MiniApi.Application.Auth.Request;

public class SignInRequest
{
    [Required]
    public string Name { get; set; } = default!;
    
    [Required]
    public string Password { get; set; } = default!;
}