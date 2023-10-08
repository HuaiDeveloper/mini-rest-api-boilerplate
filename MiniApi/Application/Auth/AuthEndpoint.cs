using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniApi.Common;

namespace MiniApi.Application.Auth;

internal static class AuthEndpoint
{
    internal static IEndpointRouteBuilder MapAuthEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapGet("/sign-in", async (HttpContext httpContext) =>
            {
                try
                {
                    var user = new
                    {
                        Email = "admin@test.com",
                        Name = "admin"
                    };

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Role, AuthRole.Admin),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var validHours = 8;
                    var issuedUtc = DateTimeOffset.UtcNow;
                    var expiresUtc = issuedUtc.AddHours(validHours);

                    var authProperties = new AuthenticationProperties()
                    {
                        IsPersistent = true,
                        IssuedUtc = issuedUtc,
                        ExpiresUtc = expiresUtc
                    };

                    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    
                    return "Success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return "Fail";
                }
            })
            .WithName("SignIn")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapGet("/sign-out", async (HttpContext httpContext) =>
            {
                try
                {
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    return "Success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return "Fail";
                }
            })
            .WithName("SignOut")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}