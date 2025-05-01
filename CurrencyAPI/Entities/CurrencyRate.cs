namespace CurrencyAPI.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
