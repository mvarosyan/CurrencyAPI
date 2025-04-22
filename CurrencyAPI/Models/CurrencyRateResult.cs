namespace CurrencyAPI.Models
{
    public class CurrencyRateResult
    {
        public bool Success { get; set; }
        public decimal Rate { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
