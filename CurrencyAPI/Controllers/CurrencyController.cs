using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CurrencyAPI.Models;
using CurrencyAPI.Services;
using CurrencyAPI.Helpers;

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
            var result = await _currencyService.AssignCurrencyAsync(request.Currency, request.Value, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return Created();
        }

        [HttpGet("rate/{currency}")]
        public async Task<IActionResult> GetRate(string currency, CancellationToken cancellationToken)
        {
            if (!currency.IsValid())
            {
                return BadRequest(new { Error = "Currency code must be exactly 3 alphabetic characters." });
            }

            var result = await _currencyService.GetCurrencyAsync(currency, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return Ok(new { Currency = currency.ToUpper(), Value = result.Value.Value });
        }

        [HttpPost("fetch-and-save")]
        public async Task<IActionResult> FetchAndSaveRates(CancellationToken cancellationToken)
        {
            var result = await _currencyService.FetchAndSaveRatesAsync(cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Error);
            }

            return Ok(new { Message = "Rates successfully fetched and saved." });
        }

        [HttpGet("calculate")]
        public async Task<IActionResult> Calculate([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount, CancellationToken cancellationToken)
        {
            if (!from.IsValid() || !to.IsValid())
            {
                return BadRequest(new { Error = "Both 'from' and 'to' currencies must be exactly 3 alphabetic characters." });
            }

            if (amount <= 0)
            {
                return BadRequest(new { Error = "'amount' must be a positive number." });
            }

            var result = await _currencyService.CalculateAsync(from, to, amount, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return Ok(new { From = from.ToUpper(), To = to.ToUpper(), Amount = amount, Result = result.Value.Value });
        }

        [HttpGet("historical")]
        public async Task<IActionResult> GetHistorical([FromQuery] string currency, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, CancellationToken cancellationToken)
        {
            if (!currency.IsValid())
            {
                return BadRequest(new { Error = "Currency code must be exactly 3 alphabetic characters." });
            }

            var result = await _currencyService.GetHistoricalAsync(currency, fromDate, toDate, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteCurrency([FromQuery] string currency, CancellationToken cancellationToken)
        {
            if (!currency.IsValid())
            {
                return BadRequest(new { Error = "Currency code must be exactly 3 alphabetic characters." });
            }

            var result = await _currencyService.DeleteCurrencyAsync(currency, cancellationToken);

            if (!result.IsSuccess)
                return NotFound(new { Error = result.Error });

            return NoContent();
        }

    }

}
