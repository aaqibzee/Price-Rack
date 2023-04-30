using Microsoft.Extensions.Configuration;

namespace PriceRack.Common
{
    public class PriceRackApiSettings 
    {
        public string BitstampEndpoint { get; set; }
        public string BitfinexEndpoint { get; set; }
        public string DataSource { get; set; }

        public PriceRackApiSettings(IConfiguration configuration)
        {
            BitstampEndpoint = configuration.GetValue<string>("PriceService:BitstampEndpoint");
            BitfinexEndpoint = configuration.GetValue<string>("PriceService:BitfinexEndpoint");
            DataSource = configuration.GetValue<string>("ConnectionStrings:PriceContext");
        }
    }
}
