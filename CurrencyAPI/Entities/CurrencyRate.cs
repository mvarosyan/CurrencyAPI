namespace CurrencyAPI.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime LastUpdated { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;
    }
}
