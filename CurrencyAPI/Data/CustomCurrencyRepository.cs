
using CurrencyAPI.Entities;
using CurrencyAPI.Models;
using CurrencyAPI.Cache;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace CurrencyAPI.Data
{
    public class CustomCurrencyRepository : ICustomCurrencyRepository
    {
        private readonly AppDbContext _context;
        private readonly ICacheService _cacheService;

        public CustomCurrencyRepository(AppDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task SaveRatesAsync(Dictionary<string, decimal> rates, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var newRates = rates.Select(rate => new CurrencyRate
            {
                Currency = rate.Key,
                Value = rate.Value,
                LastUpdated = DateTime.UtcNow
            }).ToList();

            _context.CurrencyRates.AddRange(newRates);

            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AssignAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _context.CurrencyRates.Add(new CurrencyRate
            {
                Currency = currency,
                Value = value,
                LastUpdated = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<CurrencyRate?> GetAsync(string currency, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var cacheKey = $"currency_{currency}";

            var cachedRate = _cacheService.Get<CurrencyRate>(cacheKey);

            if (cachedRate != null)
            {
                return cachedRate;
            }

            var rate = await _context.CurrencyRates
                .AsNoTracking()
                .Where(c => c.Currency == currency)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (rate != null)
            {
                _cacheService.Set(cacheKey, rate, TimeSpan.FromHours(1));
            }

            return rate;
        }

        public async Task<IEnumerable<CurrencyRate>> GetHistoricalAsync(string currency, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var cacheKey = $"historical_{currency}_{fromDate:yyyyMMddHHmmss}_{toDate:yyyyMMddHHmmss}";

            var cachedRates = _cacheService.Get<IEnumerable<CurrencyRate>>(cacheKey);

            if (cachedRates != null)
            {
                return cachedRates;
            }

            var rates = await _context.CurrencyRates
                .AsNoTracking()
                .Where(c => c.Currency == currency && c.LastUpdated >= fromDate && c.LastUpdated <= toDate)
                .OrderByDescending(c => c.Id)
                .ToListAsync(cancellationToken);

            _cacheService.Set(cacheKey, rates, TimeSpan.FromHours(1));

            return rates;
        }
    }
}
