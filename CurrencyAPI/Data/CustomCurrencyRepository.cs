
using CurrencyAPI.Models;

namespace CurrencyAPI.Data
{
    public class CustomCurrencyRepository : ICustomCurrencyRepository
    {
        private readonly Dictionary<string, decimal> _currencies = new();

        public Task AssignAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _currencies[currency.ToUpper()] = value;
            return Task.CompletedTask;
        }

        public Task<CustomCurrencyResult> GetAsync(string currency, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            if (_currencies.TryGetValue(currency, out var value))
            {
                return Task.FromResult(new CustomCurrencyResult
                {
                    Found = true,
                    Value = value
                });
            }

            return Task.FromResult(new CustomCurrencyResult
            {
                Found = false,
                Value = 0m
            });
        }
    }
}
