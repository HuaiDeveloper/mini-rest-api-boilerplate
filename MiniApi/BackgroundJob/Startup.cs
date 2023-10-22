namespace MiniApi.BackgroundJob;

public static class Startup
{
    internal static IServiceCollection AddBackgroundJob(this IServiceCollection services, IConfiguration config)
    {
        var backgroundJobSetting = config.GetSection(nameof(BackgroundJobSetting)).Get<BackgroundJobSetting>();

        if (backgroundJobSetting?.EnableJob is true) 
            services.AddHostedService<ProductStatisticsBackgroundService>();
        
        return services;
    }
}