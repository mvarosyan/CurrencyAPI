namespace CurrencyAPI.Entities
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public ICollection<CurrencyRate> Rates { get; set; } = new List<CurrencyRate>();
    }
}
