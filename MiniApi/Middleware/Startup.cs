namespace MiniApi.Middleware;

internal static class Startup
{
    internal static IServiceCollection AddMiddleware(this IServiceCollection services)
    {
        services.AddScoped<ExceptionMiddleware>();
        
        return services;
    }

    internal static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }
}