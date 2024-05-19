using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.BackgroundJob;

public class ProductStatisticsBackgroundService(
    ILogger<ProductStatisticsBackgroundService> logger,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly int _taskMinuteDelay = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    GetProductCountStatistics();

                    var millisecondsDelay = _taskMinuteDelay * 60 * 1000;
                    await Task.Delay(millisecondsDelay, stoppingToken);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"{nameof(ProductStatisticsBackgroundService)} - while loop worker exception";
                    logger.LogError(exception: ex, message: errorMessage);
                }
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(ProductStatisticsBackgroundService)} - ExecuteAsync exception";
            logger.LogError(exception: ex, message: errorMessage);
        }
    }

    private void GetProductCountStatistics()
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var utcNow = DateTime.UtcNow;
        var productCount = dbContext.Products.Count();

        var message = $"Product count: {productCount}, UTC time: {utcNow:yyyy/MM/dd HH:mm:ss}";
        logger.LogInformation(message);
    }
}