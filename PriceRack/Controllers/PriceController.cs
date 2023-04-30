
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using PriceMicroservice.Services;
using PriceRack.Services;

namespace PriceRack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceController : ControllerBase
    {
        private readonly ILogger<PriceController> _logger;
        private readonly IPriceService _priceService;

        public PriceController(ILogger<PriceController> logger, IPriceService priceService)
        {
            _logger = logger;
            _priceService = priceService;
        }

        [HttpGet("price")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<decimal>> GetAggregatedPrice([FromQuery, Description("The date and time value, with minutes and seconds discarded.")] DateTime time)
        {
            try
            {
                var price = await _priceService.GetAggregatedPrice(time);
                return Ok(price);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<PriceDto>>> GetPriceHistory([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var prices = await _priceService.GetPrices(start, end);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price history");
                return BadRequest(ex.Message);
            }
        }
    }
}
