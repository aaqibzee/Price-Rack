namespace PriceMicroservice.Services
{
    public interface IPriceAggregationService
    {
        decimal GetAggregatedPrice(IEnumerable<decimal> prices);
    }
}
