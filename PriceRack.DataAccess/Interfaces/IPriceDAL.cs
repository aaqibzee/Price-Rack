using PriceRack.DataAccess.Entities;

namespace PriceMicroservice.DAL
{
    public interface IPriceDAL
    {
        public Task<IEnumerable<Price>> GetPricesAsync(DateTime startTime, DateTime endTime);
        public Task<int> AddPriceAsync(Price price);
        public Task<Price?> GetAggregatedPriceAsync(DateTime time);
    }
}
