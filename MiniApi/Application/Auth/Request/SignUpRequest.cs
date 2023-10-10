using System.ComponentModel.DataAnnotations;

namespace MiniApi.Application.Auth.Request;

public class SignUpRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(20)]
    public string Name { get; set; } = default!;
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    
    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    public string Password { get; set; } = default!;
    
    [MaxLength(50)]
    public string? Description { get; set; }
}