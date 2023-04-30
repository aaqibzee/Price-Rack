using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PriceRack.Common;
using PriceRack.Common.Exceptions;
using PriceRack.Common.Extensions;
using System.Net;

namespace PriceMicroservice.Services
{
    public class BitFinexApiService : IBitFinexApiService
    {
        private readonly IHttpsClientWrapperService _httpsClientWrapper;
        private readonly PriceRackApiSettings _priceRackApiSettings;
        private readonly ILogger<BitFinexApiService> _logger;
        public BitFinexApiService(IHttpsClientWrapperService httpsClientWrapper, PriceRackApiSettings priceRackApiSettings, ILogger<BitFinexApiService> logger)
        {
            _priceRackApiSettings = priceRackApiSettings;
            _httpsClientWrapper = httpsClientWrapper;
            _logger = logger;
        }
        public async Task<decimal> GetPrice(DateTime time)
        {
            try
            {
                var unixTime = time.ConvertToUnixTime();
                var startTime = (unixTime - Constants.BitfinexTimeWindow);

                var endpoint = string.Format(_priceRackApiSettings.BitfinexEndpoint, Constants.BTCInstrumentName,
                    startTime.ToString(), unixTime.ToString());

                var content = await _httpsClientWrapper.GetContentFromExternalCource(endpoint);

                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogCritical("Empty response received from API.");
                    throw new CustomInvalidException(HttpStatusCode.InternalServerError,
                        "Error occurred while fetching data from external API.");
                }

                var result = JsonConvert.DeserializeObject<JArray>(content);

                if (result == null || result.Count == 0 || result[0].Count() < 3 || result[0][2] == null)
                {
                    _logger.LogCritical("Invalid response received from API.");
                    throw new CustomInvalidException(HttpStatusCode.InternalServerError,
                        "Invalid response received from external API.");
                }

                try
                {
                    return Convert.ToDecimal(result[0][2]);
                }
                catch (FormatException)
                {
                    _logger.LogCritical("Unable to convert value to decimal");
                    throw new CustomInvalidException(HttpStatusCode.InternalServerError,
                            "Invalid response received from external API.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting price from BitsFinex API. {ex.Message}");
                throw;
            }
        }
    }
}
