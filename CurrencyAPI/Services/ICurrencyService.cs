using CurrencyAPI.Models;

namespace CurrencyAPI.Services
{
    public interface ICurrencyService
    {
        Task<ServiceResult> FetchAndSaveRatesAsync(CancellationToken cancellationToken = default);

        Task AssignCurrencyAsync(string currency, decimal value, CancellationToken cancellationToken);

        Task<CustomCurrencyResult> GetCurrencyAsync(string currency, CancellationToken cancellationToken);
    }
}
