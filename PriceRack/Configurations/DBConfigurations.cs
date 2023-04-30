using Microsoft.EntityFrameworkCore;
using PriceRack.DataAccess.DBContexts;

namespace PriceRack.Configs
{
    public static class DBConfigurations
    {
        public static void ConfigureDatabase(this IServiceCollection services)
        {
            services.AddDbContextFactory<PriceContext>(op => op.UseSqlite( "Data Source=prices.db"));
        }

        public static void MigrateToLatest(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PriceContext>();
                dbContext.Database.Migrate();
            }
        }
    }
}
