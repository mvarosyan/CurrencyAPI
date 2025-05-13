using Microsoft.EntityFrameworkCore;
using CurrencyAPI.Entities;

namespace CurrencyAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) 
            : base(options) {}

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasQueryFilter(c => c.IsActive);

            modelBuilder.Entity<CurrencyRate>()
                .HasQueryFilter(cr => cr.Currency.IsActive);
        }
    }
}
