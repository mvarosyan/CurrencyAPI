using CurrencyAPI.Configuration;
using CurrencyAPI.Data;
using CurrencyAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<ServiceResult<CustomCurrencyResult>> FetchAndSaveRatesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var client = _httpClientFactory.CreateClient();
            
            try
            {
                var response = await client.GetAsync($"https://openexchangerates.org/api/latest.json?app_id={_apiSettings.CurrencyApiKey}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResult<CustomCurrencyResult>
                    {
                        Success = false,
                        Error = "Couldn't fetch the rates.",
                        Result = null
                    };
                }

                var result = await response.Content.ReadAsStringAsync(cancellationToken);

                var rateResponse = JsonConvert.DeserializeObject<RateResponse>(result);

                if (rateResponse?.Rates == null)
                {
                    return new ServiceResult<CustomCurrencyResult>
                    {
                        Success = false,
                        Error = "Rates were not valid.",
                        Result = null
                    };
                }

                //save rates
                await _customCurrencyRepository.SaveRatesAsync(rateResponse.Rates, cancellationToken);

                return new ServiceResult<CustomCurrencyResult>
                {
                    Success = true,
                    Error = null,
                    Result = null
                };
            }
            catch(Exception ex)
            {
                return new ServiceResult<CustomCurrencyResult>
                {
                    Success = false,
                    Error = ex.Message,
                    Result = null
                };
            }
        }


        public async Task<ServiceResult<CustomCurrencyResult>> AssignCurrencyAsync(string currency, decimal value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            currency = currency.ToUpper();

            await _customCurrencyRepository.AssignAsync(currency, value, cancellationToken);

            return new ServiceResult<CustomCurrencyResult>
            {
                Success = true,
                Error = null,
                Result = null
            };
        }

        public async Task<ServiceResult<CustomCurrencyResult>> GetCurrencyAsync(string currency, CancellationToken cancellationToken)
        {
            var rate = await _customCurrencyRepository.GetAsync(currency, cancellationToken);

            if (rate != null)
            {
                return new ServiceResult<CustomCurrencyResult>
                {
                    Success = true,
                    Error = null,
                    Result = new CustomCurrencyResult { Value = rate.Value }
                };
            }

            return new ServiceResult<CustomCurrencyResult>
            {
                Success = false,
                Error = $"Currency {currency} not found.",
                Result = null
            };
        }

        public async Task<ServiceResult<CustomCurrencyResult>> CalculateAsync(string from, string to, decimal amount, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rateFrom = await _customCurrencyRepository.GetAsync(from, cancellationToken);
            var rateTo = await _customCurrencyRepository.GetAsync(to, cancellationToken);

            if (rateFrom == null || rateTo == null)
            {
                return new ServiceResult<CustomCurrencyResult>
                {
                    Success = false,
                    Error = "One or more currencies not found.",
                    Result = null
                };
            }

            var convertedAmount = (amount / rateFrom.Value) * rateTo.Value;

            return new ServiceResult<CustomCurrencyResult>
            {
                Success = true,
                Error = null,
                Result = new CustomCurrencyResult { Value = convertedAmount }
            };

        }


        public async Task<ServiceResult<IEnumerable<HistoricalRate>>> GetHistoricalAsync(string currency, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            fromDate = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);
            toDate = DateTime.SpecifyKind(toDate, DateTimeKind.Utc);

            var rates = await _customCurrencyRepository.GetHistoricalAsync(currency, fromDate, toDate, cancellationToken);

            if (!rates.Any())
            {
                return new ServiceResult<IEnumerable<HistoricalRate>>
                {
                    Success = false,
                    Error = "Historical rates not found.",
                    Result = null
                };
            }

            return new ServiceResult<IEnumerable<HistoricalRate>>
            {
                Success = true,
                Error = null,
                Result = rates.Select(r => new HistoricalRate
                {
                    Currency = r.Currency,
                    Value = r.Value,
                    LastUpdated = r.LastUpdated
                })
            };
        }
    }
}
