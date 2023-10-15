using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniApi.Persistence.EntityFrameworkCore;
using MiniApi.Persistence.MongoDBDriver;

namespace MiniApi.Persistence;

internal static class Startup
{
    internal static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services
            .AddOptions<DatabaseSetting>()
            .BindConfiguration(nameof(DatabaseSetting))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services
            .AddOptions<MongoDBSetting>()
            .BindConfiguration(nameof(MongoDBSetting))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>((p, b) =>
        {
            DatabaseSetting databaseSetting = p.GetRequiredService<IOptions<DatabaseSetting>>().Value;
            b.UseNpgsql(databaseSetting.ConnectionString);
        });

        services.AddScoped<MongoDBContext>();

        return services;
    }
}