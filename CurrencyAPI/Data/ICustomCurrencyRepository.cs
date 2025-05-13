using CurrencyAPI.Entities;
using CurrencyAPI.Models;

namespace CurrencyAPI.Data
{
    public interface ICustomCurrencyRepository
    {
        Task SaveRatesAsync(Dictionary<string, decimal> rates, CancellationToken cancellationToken);
        Task<bool> AssignAsync(string currency, decimal value, CancellationToken cancellationToken);
        Task<CurrencyRate?> GetAsync(string currency, CancellationToken cancellationToken);
        Task<IEnumerable<CurrencyRate>> GetHistoricalAsync(string currency, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
        Task<bool> DeleteCurrencyAsync(string currency, CancellationToken cancellationToken);
    }
}
