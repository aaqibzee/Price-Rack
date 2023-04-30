using PriceRack.DataAccess.Entities;
using PriceRack.Services;

namespace PriceMicroservice.Services
{
    public interface IPriceService
    {
        Task<PriceModel> GetAggregatedPrice(DateTime time);
        Task<IEnumerable<PriceModel>> GetPrices(DateTime startTime, DateTime endTime);
    }
}
