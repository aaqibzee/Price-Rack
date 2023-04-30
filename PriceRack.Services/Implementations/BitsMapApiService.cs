using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PriceRack.Common;
using PriceRack.Common.Exceptions;
using PriceRack.Common.Extensions;
using PriceRack.Services.Dtos;
using System.Net;

namespace PriceMicroservice.Services
{
    public class BitsMapApiService : IBitsMapApiService
    {
        private readonly IHttpsClientWrapperService _httpsClientWrapper;
        private readonly ILogger<BitsMapApiService> _logger;
        private readonly PriceRackApiSettings _priceRackApiSettings;

        public BitsMapApiService(IHttpsClientWrapperService httpsClientWrapper, PriceRackApiSettings  priceRackApiSettings, ILogger<BitsMapApiService> logger)
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
                var endpoint = string.Format(_priceRackApiSettings.BitstampEndpoint, Constants.BTCInstrumentName.ToLower(), unixTime.ToString());
                var content = await _httpsClientWrapper.GetContentFromExternalCource(endpoint);

                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogCritical("Empty response received from API.");
                    throw new CustomInvalidException(HttpStatusCode.InternalServerError,
                        "Error occurred while fetching data from external API.");
                }

                var result = JsonConvert.DeserializeObject<BitstampAPIresponse>(content);

                if (result?.data?.ohlc == null || !result.data.ohlc.Any())
                {
                    throw new CustomInvalidException(HttpStatusCode.InternalServerError,
                        "Error occurred while fetching data from external API.");
                }

                var closePrice = result.data.ohlc[0].close;
                return ConvertToDecimal(closePrice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting price from Bitstamp API. {ex.Message}");
                throw;
            }
        }
        private decimal ConvertToDecimal(string value)
        {
            if (!decimal.TryParse(value, out var result))
            {
                _logger.LogCritical("Unable to parse data to decimal data type");
                throw new CustomInvalidException(HttpStatusCode.InternalServerError,
                        "Error occurred while fetching data from external API.");
            }

            return result;
        }
    }
}
