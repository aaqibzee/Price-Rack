using Microsoft.Extensions.Logging;
using PriceRack.Common.Exceptions;
using System.Net;

namespace PriceMicroservice.Services
{
    public class HttpsClientWrapperService: IHttpsClientWrapperService
    {
        private readonly ILogger<HttpsClientWrapperService> _logger;
        public HttpsClientWrapperService(ILogger<HttpsClientWrapperService> logger)
{
            _logger = logger;
        }
        public async Task<string> GetContentFromExternalCource(string endpoint)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to get price from API with status code {response.StatusCode}");
                    throw new CustomInvalidException(HttpStatusCode.InternalServerError,
                       "Error occurred while fetching data from external API.");
                }
                return await response.Content.ReadAsStringAsync(); ;
            }
        }
    }
}