namespace CurrencyAPI.Models
{
    public class HistoricalRate
    {
        public required string Currency { get; set; }
        public decimal Value { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
