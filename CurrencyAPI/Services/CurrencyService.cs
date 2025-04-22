using CurrencyAPI.Configuration;
using CurrencyAPI.Data;
using CurrencyAPI.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CurrencyAPI.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private readonly ICustomCurrencyRepository _customCurrencyRepository;

        public CurrencyService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ICustomCurrencyRepository customCurrencyRepository)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _customCurrencyRepository = customCurrencyRepository;
        }

        public async Task<CurrencyRateResult> GetRateAsync(string targetCurrency, CancellationToken cancellationToken)
        {

            cancellationToken.ThrowIfCancellationRequested();

            targetCurrency = targetCurrency.ToUpper();

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"https://openexchangerates.org/api/latest.json?app_id={_apiSettings.CurrencyApiKey}");

            if (!response.IsSuccessStatusCode)
            {
                return new CurrencyRateResult
                {
                    Success = false,
                    Rate = 0,
                    ErrorMessage = "Failed to retrieve data."
                };
            }

            var result = await response.Content.ReadAsStringAsync();

            var rateResponse = JsonConvert.DeserializeObject<RateResponse>(result);

            if (rateResponse == null || !rateResponse.Rates.ContainsKey(targetCurrency))
            {
                return new CurrencyRateResult
                {
                    Success = false,
                    Rate = 0,
                    ErrorMessage = "Currency not found."
                };
            }

            return new CurrencyRateResult
            {
                Success = true,
                Rate = rateResponse.Rates[targetCurrency],
                ErrorMessage = null
            };
        }

        public Task AssignCurrencyAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            return _customCurrencyRepository.AssignAsync(currency, value, cancellationToken);
        }

        public Task<CustomCurrencyResult> GetCustomCurrencyAsync(string currency, CancellationToken cancellationToken)
        {
            return _customCurrencyRepository.GetAsync(currency, cancellationToken);
        }
    }
}
