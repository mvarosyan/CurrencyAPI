namespace CurrencyAPI.Models
{
    public class RateResponse
    {
        public required Dictionary<string, decimal> Rates { get; set; }
    }
}
