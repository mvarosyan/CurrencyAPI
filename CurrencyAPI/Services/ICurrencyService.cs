using CurrencyAPI.Models;

namespace CurrencyAPI.Services
{
    public interface ICurrencyService
    {
        Task<ServiceResult> FetchAndSaveRatesAsync(CancellationToken cancellationToken = default);

        Task<ServiceResult> AssignCurrencyAsync(string currency, decimal value, CancellationToken cancellationToken);

        Task<ServiceResult<CustomCurrencyResult>> GetCurrencyAsync(string currency, CancellationToken cancellationToken);

        Task<ServiceResult<CustomCurrencyResult>> CalculateAsync(string from, string to, decimal amount, CancellationToken cancellationToken);
        Task<ServiceResult<IEnumerable<HistoricalRate>>> GetHistoricalAsync(string currency, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken);
        Task<ServiceResult> DeleteCurrencyAsync(string currency, CancellationToken cancellationToken);
    }
}
