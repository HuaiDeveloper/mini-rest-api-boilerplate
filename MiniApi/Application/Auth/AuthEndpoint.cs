using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MiniApi.Application.Auth.Request;

namespace MiniApi.Application.Auth;

internal static class AuthEndpoint
{
    internal static IEndpointRouteBuilder MapAuthEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapPost("/sign-up", async (
                [FromBody] SignUpRequest request,
                [FromServices] StaffManager staffManager) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(request.Name))
                        throw new Exception("Name required");
        
                    if (string.IsNullOrEmpty(request.Email))
                        throw new Exception("Email required");
        
                    if (string.IsNullOrEmpty(request.Password))
                        throw new Exception("Password required");

                    var isExistStaff = await staffManager.IsExistStaffNameAsync(request.Name);
                    if (isExistStaff)
                        throw new Exception("User name exist!");
                    
                    await staffManager.CreateUserStaffAsync(
                        request.Name,
                        request.Email,
                        request.Password,
                        request.Description);
                    
                    return "Success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return "Fail";
                }
            })
            .WithName("SignUp")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapPost("/sign-in", async (
                HttpContext httpContext,
                [FromBody] SignInRequest request,
                [FromServices] StaffManager staffManager) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(request.Name))
                        throw new Exception("Staff name required");
            
                    if (string.IsNullOrEmpty(request.Password))
                        throw new Exception("password required");

                    var staff = await staffManager.FindStaffByNameAsync(request.Name);
                    var verifyPasswordResult = await staffManager.VerifyPasswordAsync(staff, request.Password);
                    if (verifyPasswordResult == false)
                        throw new Exception("Verification failed");
            
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, staff.Name),
                        new Claim(ClaimTypes.Role, staff.AuthRole),
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
            .MapPost("/sign-out", async (HttpContext httpContext) =>
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