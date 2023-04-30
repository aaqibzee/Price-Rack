using Microsoft.Extensions.Logging;
using PriceMicroservice.DAL;
using PriceMicroservice.Services;
using PriceRack.DataAccess.Entities;
using PriceRack.Common;
using Mapster;
using PriceRack.Common.Exceptions;
using System.Net;

namespace PriceRack.Services
{
    public class PriceService : IPriceService
    {
        private readonly IPriceDAL _priceDAL;
        private readonly ILogger<PriceService> _logger;
        private readonly IPriceAggregationService _priceAggergationService;
        private readonly IBitsMapApiService _bitsMapApiService;
        private readonly IBitFinexApiService _bitsFinexApiService;

        public PriceService(ILogger<PriceService> logger, IPriceDAL priceDAL, IPriceAggregationService priceAggergationService,
          IBitsMapApiService bitsMapApiService, IBitFinexApiService bitsFinexApiService)
        {
            _logger = logger;
            _priceDAL = priceDAL;
            _bitsMapApiService = bitsMapApiService;
            _bitsFinexApiService = bitsFinexApiService;
            _priceAggergationService = priceAggergationService;
        }

        #region Public Methods
        /// <summary>
        /// Get aggregated price for an instrument for the provided time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task<PriceModel> GetAggregatedPrice(DateTime time)
        {
            _logger.LogInformation($"Requesting price for time {time}");

            var price = await _priceDAL.GetAggregatedPriceAsync(time);
            if (price != null)
            {
                _logger.LogInformation($"Returning aggregated price from the database for time {time}");
                return price.Adapt<PriceModel>();
            }

            var aggerigatedPrice= await GetPrice(time);
            return new PriceModel
            {
                Time = time,
                Instrument = Constants.BTCInstrumentName,
                Value = aggerigatedPrice
            };
        }
        /// <summary>
        /// Get prices from DB
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PriceModel>> GetPrices(DateTime startTime, DateTime endTime)
        {
            var prices = await _priceDAL.GetPricesAsync(startTime, endTime);
            return prices.Adapt<IEnumerable<PriceModel>>();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get price from external soure, save in the DB and return the price
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private async Task<decimal> GetPrice(DateTime time)
        {
            var prices = await GetPricesFromExternalSources(time);
            var aggregatedPrice = _priceAggergationService.GetAggregatedPrice(prices);

            await _priceDAL.AddPriceAsync(new Price(time, aggregatedPrice, Constants.BTCInstrumentName));

            _logger.LogInformation($"Persisted aggregated price {aggregatedPrice} for time {time}");
            return aggregatedPrice;
        }
        /// <summary>
        ///  Gets prices from external sources
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <exception cref="CustomInvalidException"></exception>
        private async Task<decimal[]> GetPricesFromExternalSources(DateTime time)
        {
            try
            {
                var sources = new List<Task<decimal>>()
                {
                     _bitsMapApiService.GetPrice(time),
                     _bitsFinexApiService.GetPrice(time),
                };

                return (await Task.WhenAll(sources));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving prices from external APIs ", ex);
                throw new CustomInvalidException(HttpStatusCode.ServiceUnavailable, "External service unavailable, try later");
            }
        }
        #endregion
    }
}