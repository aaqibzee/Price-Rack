
using Mapster;
using Microsoft.AspNetCore.Mvc;
using PriceMicroservice.Services;
using PriceRack.Attributes;
using PriceRack.Common.Extensions;
using PriceRack.DataAccess.Entities;
using PriceRack.Services;

namespace PriceRack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [GeneralExceptionFilter]
    public class PricesController : ControllerBase
    {
        private readonly ILogger<PricesController> _logger;
        private readonly IPriceService _priceService;

        public PricesController(ILogger<PricesController> logger, IPriceService priceService)
        {
            _logger = logger;
            _priceService = priceService;
        }

        [HttpGet("price")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PriceDTO>> GetAggregatedPrice([FromQuery][NotInFuture("Time")] DateTime time)
        {
            _logger.LogInformation($"Price reqeusted ofr {time}");

            var aggerigatedPrice = await _priceService.GetAggregatedPrice(time.RoundHour());
            var response = aggerigatedPrice.Adapt<PriceDTO>();
            return Ok(response);
        }

        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PriceModel>>> GetPriceHistory([FromQuery][NotInFuture("Start time")] DateTime start, [FromQuery] DateTime end)
        {
            _logger.LogInformation($"Price reqeusted between {start} and {end}");

            if (start > end)
            {
                return BadRequest("Start time cannot be greater than end time.");
            }

            var prices = await _priceService.GetPrices(start.RoundHour(), end.RoundHour());
            var priceDtos = prices.Adapt<IEnumerable<PriceDTO>>();
            return Ok(priceDtos);
        }
    }
}
