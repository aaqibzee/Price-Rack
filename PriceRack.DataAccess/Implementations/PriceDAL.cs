using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PriceMicroservice.DAL;
using PriceRack.Common;
using PriceRack.DataAccess.DBContexts;
using PriceRack.DataAccess.Entities;

namespace PriceRack.DAL
{
    public class PriceDAL : IPriceDAL
    {
        private readonly ILogger<PriceDAL> _logger;
        private readonly IDbContextFactory<PriceContext> _contextFactory;

        public PriceDAL(ILogger<PriceDAL> logger, IDbContextFactory<PriceContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }
        #region Public Methods
        public async Task<int> AddPriceAsync(Price price)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                try
                {
                    context.Prices.Add(price);
                    return await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError("Error saving price to database", ex);
                    throw new Exception("Error saving price to database", ex);
                }
            }
        }
        public async Task<IEnumerable<Price>> GetPricesAsync(DateTime startTime, DateTime endTime)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var prices = await context.Prices
                        .Where(p => p.Instrument == Constants.BTCInstrumentName && p.Time >= startTime && p.Time <= endTime)
                        .ToListAsync();

                    return prices;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error retrieving prices from database", ex);
                    throw new Exception("Error retrieving prices from database", ex);
                }
            }
        }
        public async Task<Price?> GetAggregatedPriceAsync(DateTime time)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                try
                {
                    var price = await context.Prices.FindAsync(Constants.BTCInstrumentName, time);
                    if (price != null)
                    {
                        _logger.LogInformation($"Returning aggregated price from the database for time {time}");
                        return price;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error retrieving aggregated price from database", ex);
                    throw new Exception("Error retrieving aggregated price from database", ex);
                }
            }
        }
        #endregion
    }
}
