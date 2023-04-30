using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PriceMicroservice.Services;
using PriceRack.DataAccess;
using PriceRack.DataAccess.Entities;
using PriceRack.Services.Dtos;

namespace PriceRack.Services
{
    public class PriceService : IPriceService
    {
        private readonly ILogger<PriceService> _logger;
        private readonly IConfiguration _configuration;
        private readonly PriceContext _context;
        private readonly IDbContextFactory<PriceContext> _contextFactory;

        string bitstampEndpoint;
        string bitfinexEndpoint;

        public PriceService(ILogger<PriceService> logger, IConfiguration configuration, IDbContextFactory<PriceContext> contextFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _contextFactory = contextFactory;

            bitstampEndpoint = _configuration["PriceService:BitstampEndpoint"];
            bitfinexEndpoint = _configuration["PriceService:BitfinexEndpoint"];
        }

        public async Task AddPriceAsync(Price price)
        {
            _context.Prices.Add(price);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetAggregatedPrice(DateTime time)
        {
            _logger.LogInformation($"Requesting price for time {time}");
            time = time.Date.AddHours(time.Hour);

            // Try getting price from database
            using (var context = _contextFactory.CreateDbContext())
            {
                var price = await context.Prices.FindAsync(Constants.BTCInstrumentName, time);
                if (price != null)
                {
                    _logger.LogInformation($"Returning aggregated price from the database for time {time}");
                    return price.Value;
                }
            }

            // If not available in database, fetch from external sources, aggregate and save in database
            var sources = new List<Task<decimal>>()
            {
                GetPriceFromBitstamp(time),
                GetPriceFromBitfinex(time)
            };

            var aggregatedPrice = (await Task.WhenAll(sources)).Average();

            using (var context = _contextFactory.CreateDbContext())
            {
                var newPrice = new Price
                {
                    Instrument = Constants.BTCInstrumentName,
                    Time = time,
                    Value = aggregatedPrice
                };

                context.Prices.Add(newPrice);
                await context.SaveChangesAsync();
                _logger.LogInformation($"Persisted aggregated price {aggregatedPrice} for time {time}");
            }

            _logger.LogInformation($"Returning aggregated price {aggregatedPrice} for time {time}");
            return aggregatedPrice;
        }
        private async Task<decimal> GetPriceFromBitstamp(DateTime time)
        {
            var unixTime = ConvertToUnixTime(time);
            var endpoint = string.Format(bitstampEndpoint, Constants.BTCInstrumentName.ToLower(), unixTime.ToString());
            return await GetPriceFrommBBitstampSource(endpoint);
        }

        public async Task<IEnumerable<Price>> GetPrices(DateTime startTime, DateTime endTime)
        {
            startTime = startTime.Date.AddHours(startTime.Hour);
            endTime = endTime.Date.AddHours(endTime.Hour);
            using (var context = _contextFactory.CreateDbContext())
            {
                var prices = await context.Prices
                    .Where(p => p.Instrument == Constants.BTCInstrumentName && p.Time >= startTime && p.Time <= endTime)
                    .ToListAsync();

                return prices;
            }
        }


        private async Task<decimal> GetPriceFromBitfinex(DateTime time)
        {
            var unixTime = ConvertToUnixTime(time);
            var startTime = (unixTime - Constants.BitfinexTimeWindow);
            var endpoint = string.Format(bitfinexEndpoint,Constants.BTCInstrumentName,
                startTime.ToString(), unixTime.ToString());

            return await GetPriceFrommBitfinexSource(endpoint);
        }
        private async Task<decimal> GetPriceFrommBBitstampSource(string endpoint)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get price from Bitstamp with status code {response.StatusCode}.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<BitstampAPIresponse>(content);
                var closePrice = Convert.ToDecimal(result.data.ohlc[0].close);

                return closePrice;
            }
        }

        private async Task<decimal> GetPriceFrommBitfinexSource(string endpoint)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get price from Bitfinex with status code {response.StatusCode}.");
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<JArray>(content);
                return Convert.ToDecimal(result[0][2]);
            }
        }

        private long ConvertToUnixTime(DateTime dateTime)
        {
            var unixEpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixEpochTime).Ticks;
            var unixTimeStampInMilliseconds = unixTimeStampInTicks / TimeSpan.TicksPerMillisecond;
            return unixTimeStampInMilliseconds / 1000;
        }
    }
}
