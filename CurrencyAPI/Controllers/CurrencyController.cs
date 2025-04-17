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
        public async Task<IActionResult> GetRate(string targetCurrency)
        {
            var (success, rate, error) = await _currencyService.GetRateAsync(targetCurrency);

            if (!success)
                return NotFound(error);

            return Ok(new { Currency = targetCurrency.ToUpper(), Rate = rate });

        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignCurrency([FromBody] AssignCurrencyRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _currencyService.AssignCurrencyAsync(request.Currency, request.Value);

            return Ok();
        }

        [HttpGet("custom/{currency}")]
        public async Task<IActionResult> GetCustomCurrency(string currency)
        {
            var (found, value) = await _currencyService.GetCustomCurrencyAsync(currency);

            if (!found)
                return NotFound(new { Error = $"Currency '{currency.ToUpper()}' not found." });

            return Ok(new { Currency = currency.ToUpper(), Value = value });
        }
    }

    
}
