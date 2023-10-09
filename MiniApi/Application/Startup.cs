using Microsoft.AspNetCore.Authentication.Cookies;
using MiniApi.Application.Auth;
using MiniApi.Application.Products;
using MiniApi.Common;

namespace MiniApi.Application;

internal static class Startup
{
    internal static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
            .MapProductEndpoint()
            .MapAuthEndpoint();
        
        return endpointRouteBuilder;
    }
    internal static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = ".MiniApi.Cookies";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = true;
                options.EventsType = typeof(CustomCookieAuthenticationEvents);
            });

        services.AddAuthorization(options =>
        {
            string[] authRoles = AuthRole.GetAuthRoles();
            foreach (var role in authRoles)
            {
                options.AddPolicy(role, (policy) => policy.RequireRole(role));
            }
        });
        
        services
            .AddScoped<CustomCookieAuthenticationEvents>()
            .AddScoped<AuthService>()
            .AddScoped<StaffManager>()
            .AddScoped<ProductService>();
        
        return services;
    }
    
    internal static IApplicationBuilder UseApplication(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}