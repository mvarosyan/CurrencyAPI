
using CurrencyAPI.Entities;
using CurrencyAPI.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            List<Currency> currencies = new List<Currency>();

            if (!_context.Currencies.Any())
            {
                var newCurrencies = rates.Keys.Select(code => new Currency
                {
                    Code = code,
                    IsActive = true
                }).ToList();

                _context.Currencies.AddRange(newCurrencies);
                await _context.SaveChangesAsync(cancellationToken);
            }

            currencies = await _context.Currencies.ToListAsync(cancellationToken);

            var newRates = currencies
                .Select(cur =>
                {
                    return new CurrencyRate
                    {
                        CurrencyId = cur.Id,
                        Value = rates[cur.Code],
                        LastUpdated = DateTime.UtcNow
                    };
                })
                .ToList();

            _context.CurrencyRates.AddRange(newRates);

            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<bool> AssignAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currencyCode = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == currency, cancellationToken);

            if (currencyCode == null)
            {
                return false;
            }

            var newRate = new CurrencyRate
            {
                CurrencyId = currencyCode.Id,
                Value = value,
                LastUpdated = DateTime.UtcNow
            };

            _context.CurrencyRates.Add(newRate);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<CurrencyRate?> GetAsync(string currency, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var currencyCode = await _context.Currencies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == currency, cancellationToken);

            if (currencyCode == null)
            {
                return null;
            }

            var rate = await _context.CurrencyRates
                .AsNoTracking()
                .Where(c => c.CurrencyId == currencyCode.Id)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return rate;
        }

        public async Task<IEnumerable<HistoricalRate>> GetHistoricalAsync(string currency, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            currency = currency.ToUpper();

            var rates = await _context.CurrencyRates
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(c => c.Currency.Code == currency && c.LastUpdated >= fromDate && c.LastUpdated <= toDate)
                .OrderByDescending(c => c.Id)
                .Select(r => new HistoricalRate
                {
                    Currency = r.Currency.Code,
                    Value = r.Value,
                    LastUpdated = r.LastUpdated
                })
                .ToListAsync(cancellationToken);

            return rates;
        }

        public async Task<bool> DeleteCurrencyAsync(string currency, CancellationToken cancellationToken)
        {
            currency = currency.ToUpper();

            var currencyCode = await _context.Currencies
                .FirstOrDefaultAsync(c => c.Code == currency, cancellationToken);

            if (currencyCode == null)
            {
                return false;
            }

            currencyCode.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
