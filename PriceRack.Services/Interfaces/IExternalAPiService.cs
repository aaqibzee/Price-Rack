namespace PriceMicroservice.Services
{
    public interface IExternalAPiService
    {
        public Task<decimal> GetPrice(DateTime time);
    }
}