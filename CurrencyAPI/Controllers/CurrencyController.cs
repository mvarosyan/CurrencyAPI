using CurrencyAPI.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CurrencyAPI.Models;
using CurrencyAPI.Services;

namespace CurrencyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

   
        [HttpPost("assign")]
        public async Task<IActionResult> AssignCurrency([FromBody] AssignCurrencyRequest request, CancellationToken cancellationToken)
        {
            await _currencyService.AssignCurrencyAsync(request.Currency, request.Value, cancellationToken);

            return Ok();
        }

        [HttpGet("rate/{currency}")]
        public async Task<IActionResult> GetRate(string currency, CancellationToken cancellationToken)
        {
            var result = await _currencyService.GetCurrencyAsync(currency, cancellationToken);

            if (!result.Found)
                return NotFound(new { Error = $"Currency '{currency.ToUpper()}' not found." });

            return Ok(new { Currency = currency.ToUpper(), Value = result.Value });
        }

        [HttpPost("fetch-and-save")]
        public async Task<IActionResult> FetchAndSaveRates(CancellationToken cancellationToken)
        {
            var result = await _currencyService.FetchAndSaveRatesAsync(cancellationToken);

            if (!result.Success)
            {
                return StatusCode(500, result.Error);
            }

            return Ok(new { Message = "Rates successfully fetched and saved." });
        }
    }

}
