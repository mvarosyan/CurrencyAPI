using CurrencyAPI.Configuration;
using CurrencyAPI.Data;
using CurrencyAPI.Models;
using CurrencyAPI.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using CurrencyAPI.Entities;

namespace CurrencyAPI.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;
        private readonly ICustomCurrencyRepository _customCurrencyRepository;
        private readonly ICacheService _cacheService;

        public CurrencyService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings, ICustomCurrencyRepository customCurrencyRepository, ICacheService cacheService)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
            _customCurrencyRepository = customCurrencyRepository;
            _cacheService = cacheService;
        }

        public async Task<ServiceResult> FetchAndSaveRatesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = _httpClientFactory.CreateClient();
            
            try
            {
                var response = await client.GetAsync($"https://openexchangerates.org/api/latest.json?app_id={_apiSettings.CurrencyApiKey}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return ServiceResult.Failure("Failed to fetch rates from API.");
                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);

                var rateResponse = JsonConvert.DeserializeObject<RateResponse>(result);

                if (rateResponse?.Rates == null)
                {
                    return ServiceResult.Failure("Invalid response from API.");
                }

                //save rates
                await _customCurrencyRepository.SaveRatesAsync(rateResponse.Rates, cancellationToken);

                return ServiceResult.Success();
            }
            catch(Exception ex)
            {
                return ServiceResult.Failure($"An error occurred while fetching rates: {ex.Message}");
            }
        }


        public async Task<ServiceResult> AssignCurrencyAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var isSuccess = await _customCurrencyRepository.AssignAsync(currency, value, cancellationToken);

            if (!isSuccess)
            {
                return ServiceResult.Failure("Currency not found.");
            }

            _cacheService.Remove($"currency_{currency}");

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<CustomCurrencyResult>> GetCurrencyAsync(string currency, CancellationToken cancellationToken)
        {

            string cacheKey = $"currency_{currency.ToUpper()}";

            var cached = _cacheService.Get<CustomCurrencyResult>(cacheKey);

            if (cached != null)
            {
                return ServiceResult<CustomCurrencyResult>.Success(cached);
            }

            var rate = await _customCurrencyRepository.GetAsync(currency, cancellationToken);

            if (rate != null)
            {
                var result = new CustomCurrencyResult { Value = rate.Value };

                _cacheService.Set(cacheKey, result, TimeSpan.FromHours(1));

                return ServiceResult<CustomCurrencyResult>.Success(result);
            }

            return ServiceResult<CustomCurrencyResult>.Failure("Currency not found.");
        }

        public async Task<ServiceResult<CustomCurrencyResult>> CalculateAsync(string from, string to, decimal amount, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fromResult = await GetCurrencyAsync(from, cancellationToken);
            if (!fromResult.IsSuccess || fromResult.Value == null)
            {
                return ServiceResult<CustomCurrencyResult>.Failure("Source currency not found.");
            }

            var toResult = await GetCurrencyAsync(to, cancellationToken);
            if (!toResult.IsSuccess || toResult.Value == null)
            {
                return ServiceResult<CustomCurrencyResult>.Failure("Target currency not found.");
            }

            var convertedAmount = (amount / fromResult.Value.Value) * toResult.Value.Value;

            return ServiceResult<CustomCurrencyResult>.Success(new CustomCurrencyResult { Value = convertedAmount });

        }


        public async Task<ServiceResult<IEnumerable<HistoricalRate>>> GetHistoricalAsync(string currency, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DateTime defaultFrom = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime defaultTo = DateTime.UtcNow;

            var from = DateTime.SpecifyKind(fromDate ?? defaultFrom, DateTimeKind.Utc);
            var to = DateTime.SpecifyKind(toDate ?? defaultTo, DateTimeKind.Utc);

            var rates = await _customCurrencyRepository.GetHistoricalAsync(currency, from, to, cancellationToken);

            if (!rates.Any())
            {
                return ServiceResult<IEnumerable<HistoricalRate>>.Failure("No historical rates found for the specified currency and date range.");
            }

            return ServiceResult<IEnumerable<HistoricalRate>>.Success(rates);
        }

        public async Task<ServiceResult> DeleteCurrencyAsync(string currency, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            currency = currency.ToUpper();

            var isSuccess = await _customCurrencyRepository.DeleteCurrencyAsync(currency, cancellationToken);

            if (!isSuccess)
            {
                return ServiceResult.Failure("Currency not found.");
            }

            _cacheService.Remove($"currency_{currency}");

            return ServiceResult.Success();
        }
    }
}
