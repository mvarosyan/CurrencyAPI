using CurrencyAPI.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CurrencyAPI.Models;

namespace CurrencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public CurrencyController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        [HttpGet("rate/{targetCurrency}")]
        public async Task<IActionResult> GetRate(string targetCurrency)
        {
            targetCurrency = targetCurrency.ToUpper();

            var client = _httpClientFactory.CreateClient();
          
            var response = await client.GetAsync($"https://openexchangerates.org/api/latest.json?app_id={_apiSettings.CurrencyApiKey}");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var rateResponse = JsonConvert.DeserializeObject<RateResponse>(result);

            if (rateResponse == null || !rateResponse.Rates.ContainsKey(targetCurrency))
            {
                return NotFound("Currency not found.");
            }

            var targetRate = rateResponse.Rates[targetCurrency];

            return Ok(new { Currency = targetCurrency, Rate = targetRate });

        }
    }

    
}
