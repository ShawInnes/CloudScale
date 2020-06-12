using System;
using System.Threading.Tasks;
using CloudScale.ApiClient;
using CloudScale.Contracts.Ping;
using CloudScale.Contracts.Weather;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Serilog;

namespace CloudScale.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true)
                .AddJsonFile("appsettings.development.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            Log.Logger = new LoggingFactory(configuration)
                .CreateLogger();

            Log.Information("Starting CLI");

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            
            ICloudScaleClient client = new CloudScaleClient(new CloudScaleClientConfiguration
                {BaseUrl = "https://localhost:5001"}, jsonSerializerSettings);

            var healthResponse = await client.GetHealth();
            Log.Information("Health Response {Response}", healthResponse);

            var pingResponse = await client.GetPing(new PingRequest {Message = Guid.NewGuid().ToString("N")});
            Log.Information("Ping Response {Response}", pingResponse.Message);

            var weatherResponse = await client.GetWeatherForecast(new WeatherRequest());
            Log.Information("Weather Response {@Response}", weatherResponse);

            Log.Information("Ending CLI");

            Log.CloseAndFlush();
        }
    }
}