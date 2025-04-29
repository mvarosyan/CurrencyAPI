using CurrencyAPI.Configuration;
using CurrencyAPI.Data;
using CurrencyAPI.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ServiceResult> FetchAndSaveRatesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = _httpClientFactory.CreateClient();
            
            try
            {
                var response = await client.GetAsync($"https://openexchangerates.org/api/latest.json?app_id={_apiSettings.CurrencyApiKey}");

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Error = "Couldn't fetch the rates."
                    };
                }

                var result = await response.Content.ReadAsStringAsync();

                var rateResponse = JsonConvert.DeserializeObject<RateResponse>(result);

                if (rateResponse?.Rates == null)
                {
                    return new ServiceResult
                    {
                        Success = false,
                        Error = "Rates were not valid."
                    };
                }

                //save rates
                await _customCurrencyRepository.SaveRatesAsync(rateResponse.Rates, cancellationToken);

                return new ServiceResult
                {
                    Success = true,
                    Error = null
                };
            }
            catch(Exception ex)
            {
                return new ServiceResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }


        public Task AssignCurrencyAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            return _customCurrencyRepository.AssignAsync(currency, value, cancellationToken);
        }

        public Task<CustomCurrencyResult> GetCurrencyAsync(string currency, CancellationToken cancellationToken)
        {
            return _customCurrencyRepository.GetAsync(currency, cancellationToken);
        }
    }
}
