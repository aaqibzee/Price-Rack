namespace PriceRack.Extensions
{
    public static class LoggingConfigurations
    {
        /// <summary>
        /// Responsible for doing log configurations.
        /// </summary>
        /// <param name="repository"></param>
        public static void ConfigureLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                       .AddFilter("System", LogLevel.Warning)
                       .AddFilter("PriceRack", LogLevel.Debug)
                       .AddFile(configuration.GetValue<string>("Logging:LogFilePath"),
                        LogLevel.Information, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}");
            });
        }
    }
}
