
using CurrencyAPI.Entities;
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

            if (!_context.Currencies.Any())
            {
                var newCurrencies = rates.Keys.Select(code => new Currency
                {
                    Code = code,
                    IsActive = true
                }).ToList();

                _context.Currencies.AddRange(newCurrencies);
            }

            var newRates = rates
                .Select(rate =>
                {
                    var currency = _context.Currencies.FirstOrDefault(c => c.Code == rate.Key);
                    if (currency != null)
                    {
                        return new CurrencyRate
                        {
                            CurrencyId = currency.Id,
                            Value = rate.Value,
                            LastUpdated = DateTime.UtcNow
                        };
                    }

                    return null!;
                })
                .Where(rate => rate != null)
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

        public async Task<IEnumerable<CurrencyRate>> GetHistoricalAsync(string currency, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            currency = currency.ToUpper();

            var currencyCode = await _context.Currencies
                .AsNoTracking()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Code == currency, cancellationToken);


            var rates = await _context.CurrencyRates
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(r => r.Currency)
                .Where(c => c.CurrencyId == currencyCode!.Id && c.LastUpdated >= fromDate && c.LastUpdated <= toDate)
                .OrderByDescending(c => c.Id)
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
