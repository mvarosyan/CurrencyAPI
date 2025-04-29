using Microsoft.EntityFrameworkCore;
using CurrencyAPI.Entities;

namespace CurrencyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) 
            : base(options) {}

        public DbSet<CurrencyRate> CurrencyRates { get; set; }
    }
}
