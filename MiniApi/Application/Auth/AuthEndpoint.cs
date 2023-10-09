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
                [FromServices] AuthService authService) => await authService.SignUp(request))
            .WithName("SignUp")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapPost("/sign-in", async (
                HttpContext httpContext,
                [FromBody] SignInRequest request,
                [FromServices] AuthService authService) => await authService.SignIn(request))
            .WithName("SignIn")
            .WithOpenApi();
        
        endpointRouteBuilder
            .MapPost("/sign-out", async (
                HttpContext httpContext,
                [FromServices] AuthService authService) => await authService.SignOut())
            .WithName("SignOut")
            .WithOpenApi();

        return endpointRouteBuilder;
    }
}