using CurrencyAPI.Entities;
using CurrencyAPI.Models;

namespace CurrencyAPI.Data
{
    public interface ICustomCurrencyRepository
    {
        Task SaveRatesAsync(Dictionary<string, decimal> rates, CancellationToken cancellationToken);
        Task AssignAsync(string currency, decimal value, CancellationToken cancellationToken);
        Task<CurrencyRate?> GetAsync(string currency, CancellationToken cancellationToken);
        Task<IEnumerable<CurrencyRate>> GetHistoricalAsync(string currency, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken);
    }
}
