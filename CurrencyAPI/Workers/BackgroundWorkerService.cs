using CurrencyAPI.Services;
using Microsoft.Extensions.DependencyInjection;


namespace CurrencyAPI.Workers
{
    public class BackgroundWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<BackgroundWorkerService> _logger;

        public BackgroundWorkerService(IServiceScopeFactory serviceScopeFactory, ILogger<BackgroundWorkerService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    ICurrencyService currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();

                    var result = await currencyService.FetchAndSaveRatesAsync(stoppingToken);

                    if (result.IsSuccess)
                    {
                        _logger.LogInformation("Rates have been fetched and saved");
                    }
                    else
                    {
                        _logger.LogWarning("Currency rate fetch failed: {Error}", result.Error);
                    }

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Background Worker stopping due to cancellation.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in background worker.");
                }
            }
        }
    }
}
