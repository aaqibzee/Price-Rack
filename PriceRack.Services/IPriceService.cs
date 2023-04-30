using PriceRack.DataAccess.Entities;

namespace PriceMicroservice.Services
{
    public interface IPriceService
    {
        Task<decimal> GetAggregatedPrice(DateTime time);
        Task<IEnumerable<Price>> GetPrices(DateTime startTime, DateTime endTime);
    }
}
