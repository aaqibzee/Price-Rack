using PriceMicroservice.DAL;
using PriceMicroservice.Services;
using PriceRack.Common;
using PriceRack.DAL;
using PriceRack.Services;

namespace PriceRack.DI
{
    public static class ServiceDI
    {
        /// <summary>
        /// Extension method for resolving the service dependencies.
        /// </summary>
        /// <param name="services"></param>
         public static void RegisterDependencies(this IServiceCollection services)
        {
            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<IPriceDAL, PriceDAL>();
            services.AddScoped<IHttpsClientWrapperService, HttpsClientWrapperService>();
            services.AddSingleton<PriceRackApiSettings, PriceRackApiSettings>();
            services.AddScoped<IPriceAggregationService, PriceAggregationService>();
            services.AddScoped<IBitsMapApiService, BitsMapApiService>();
            services.AddScoped<IBitFinexApiService, BitFinexApiService>();
        }
    }
}
