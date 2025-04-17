using CurrencyAPI.Configuration;
using CurrencyAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CurrencyAPI.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private static readonly Dictionary<string, decimal> _customCurrencyValues = new();

        public CurrencyService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<(bool Success, decimal Rate, string ErrorMessage)> GetRateAsync(string targetCurrency)
        {
            targetCurrency = targetCurrency.ToUpper();

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"https://openexchangerates.org/api/latest.json?app_id={_apiSettings.CurrencyApiKey}");

            if (!response.IsSuccessStatusCode)
                return (false, 0, "Failed to retrieve data.");

            var result = await response.Content.ReadAsStringAsync();

            var rateResponse = JsonConvert.DeserializeObject<RateResponse>(result);

            if (rateResponse == null || !rateResponse.Rates.ContainsKey(targetCurrency))
                return (false, 0, "Currency not found.");


            return (true, rateResponse.Rates[targetCurrency], null);
        }

        public Task AssignCurrencyAsync(string currency, decimal value)
        {
            _customCurrencyValues[currency.ToUpper()] = value;
            return Task.CompletedTask;
        }

        public Task<(bool Found, decimal Value)> GetCustomCurrencyAsync(string currency)
        {
            currency = currency.ToUpper();

            if (_customCurrencyValues.TryGetValue(currency, out var value))
            {
                return Task.FromResult((true, value));
            }

            return Task.FromResult((false, 0m));
        }
    }
}
