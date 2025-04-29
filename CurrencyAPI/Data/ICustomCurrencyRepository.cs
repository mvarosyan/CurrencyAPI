using CurrencyAPI.Models;

namespace CurrencyAPI.Data
{
    public interface ICustomCurrencyRepository
    {
        Task SaveRatesAsync(Dictionary<string, decimal> rates, CancellationToken cancellationToken);
        Task AssignAsync(string currency, decimal value, CancellationToken cancellationToken);
        Task<CustomCurrencyResult> GetAsync(string currency, CancellationToken cancellationToken);
    }
}
