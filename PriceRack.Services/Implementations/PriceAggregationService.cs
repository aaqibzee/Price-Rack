namespace PriceMicroservice.Services
{
    public class PriceAggregationService : IPriceAggregationService
    {
       public decimal GetAggregatedPrice(IEnumerable<decimal> prices)
        {
            return prices.Average();
        }
    }
}
