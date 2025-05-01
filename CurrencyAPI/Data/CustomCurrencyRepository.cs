
using CurrencyAPI.Entities;
using CurrencyAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace CurrencyAPI.Data
{
    public class CustomCurrencyRepository : ICustomCurrencyRepository
    {
        private readonly AppDbContext _context;

        public CustomCurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        private Task UpdateRates(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _context.CurrencyRates.Add(new CurrencyRate
            {
                Currency = currency,
                Value = value,
                LastUpdated = DateTime.UtcNow
            });

            return Task.CompletedTask;
        }

        public async Task SaveRatesAsync(Dictionary<string, decimal> rates, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var rate in rates)
            {
                var currency = rate.Key;
                var value = rate.Value;

                await UpdateRates(currency, value, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task AssignAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await UpdateRates(currency, value, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<CurrencyRate?> GetAsync(string currency, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var rate = await _context.CurrencyRates
                .AsNoTracking()
                .Where(c => c.Currency == currency)
                .OrderByDescending(c => c.LastUpdated)
                .FirstOrDefaultAsync(cancellationToken);

            return rate;
        }
    }
}
