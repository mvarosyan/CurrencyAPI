
using CurrencyAPI.Entities;
using CurrencyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyAPI.Data
{
    public class CustomCurrencyRepository : ICustomCurrencyRepository
    {
        private readonly AppDbContext _context;

        public CustomCurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveRatesAsync(Dictionary<string, decimal> rates, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var rate in rates)
            {
                var currency = rate.Key.ToUpper();
                var value = rate.Value;

                var existing = await _context.CurrencyRates.FirstOrDefaultAsync(c => c.Currency == currency, cancellationToken);

                if (existing != null)
                {
                    existing.Value = value;
                }
                else
                {
                    _context.CurrencyRates.Add(new CurrencyRate
                    {
                        Currency = currency,
                        Value = value,
                    });
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AssignAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var existing = await _context.CurrencyRates.FirstOrDefaultAsync(c => c.Currency == currency, cancellationToken);

            if (existing != null)
            {
                existing.Value = value;
            }
            else
            {
                _context.CurrencyRates.Add(new CurrencyRate
                {
                    Currency = currency,
                    Value = value,
                });
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<CustomCurrencyResult> GetAsync(string currency, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var rate = await _context.CurrencyRates.FirstOrDefaultAsync(c => c.Currency == currency, cancellationToken);

            if (rate != null)
            {
                return new CustomCurrencyResult
                {
                    Found = true,
                    Value = rate.Value
                };
            }

            return new CustomCurrencyResult
            {
                Found = false,
                Value = 0m
            };
        }
    }
}
