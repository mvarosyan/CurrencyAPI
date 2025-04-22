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

        [HttpGet("rate/{targetCurrency}")]
        public async Task<IActionResult> GetRate(string targetCurrency, CancellationToken cancellationToken)
        {
            var result = await _currencyService.GetRateAsync(targetCurrency, cancellationToken);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(new { Currency = targetCurrency.ToUpper(), Rate = result.Rate });

        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignCurrency([FromBody] AssignCurrencyRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _currencyService.AssignCurrencyAsync(request.Currency, request.Value, cancellationToken);

            return Ok();
        }

        [HttpGet("custom/{currency}")]
        public async Task<IActionResult> GetCustomCurrency(string currency, CancellationToken cancellationToken)
        {
            var result = await _currencyService.GetCustomCurrencyAsync(currency, cancellationToken);

            if (!result.Found)
                return NotFound(new { Error = $"Currency '{currency.ToUpper()}' not found." });

            return Ok(new { Currency = currency.ToUpper(), Value = result.Value });
        }
    }

    
}
