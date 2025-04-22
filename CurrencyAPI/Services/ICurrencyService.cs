using CurrencyAPI.Models;

namespace CurrencyAPI.Services
{
    public interface ICurrencyService
    {
        Task<CurrencyRateResult> GetRateAsync(string targetCurrency, CancellationToken cancellationToken);

        Task AssignCurrencyAsync(string currency, decimal value, CancellationToken cancellationToken);

        Task<CustomCurrencyResult> GetCustomCurrencyAsync(string currency, CancellationToken cancellationToken);
    }
}
