namespace CurrencyAPI.Services
{
    public interface ICurrencyService
    {
        Task<(bool Success, decimal Rate, string ErrorMessage)> GetRateAsync(string targetCurrency);

        Task AssignCurrencyAsync(string currency, decimal value);

        Task<(bool Found, decimal Value)> GetCustomCurrencyAsync(string currency);
    }
}
