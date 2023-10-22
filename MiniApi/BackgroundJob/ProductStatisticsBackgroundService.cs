using MiniApi.Persistence.EntityFrameworkCore;

namespace MiniApi.BackgroundJob;

public class ProductStatisticsBackgroundService : BackgroundService
{
    private readonly ILogger<ProductStatisticsBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly int _taskMinuteDelay = 3;
    
    public ProductStatisticsBackgroundService(
        ILogger<ProductStatisticsBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

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
                    _logger.LogError(exception: ex, message: errorMessage);
                }
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"{nameof(ProductStatisticsBackgroundService)} - ExecuteAsync exception";
            _logger.LogError(exception: ex, message: errorMessage);
        }
    }

    private void GetProductCountStatistics()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var utcNow = DateTime.UtcNow;
        var productCount = dbContext.Products.Count();

        var message = $"Product count: {productCount}, UTC time: {utcNow:yyyy/MM/dd HH:mm:ss}";
        _logger.LogInformation(message);
    }
}