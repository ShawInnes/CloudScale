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

            var bearerToken =
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL2Nsb3Vkc2NhbGVkZW1vLmIyY2xvZ2luLmNvbS81ZGExMjVjNC1hMmE1LTQ5OTMtOWZkYS0xZDg2YmJmNDgxOTMvdjIuMC8iLCJleHAiOjE1OTE5NzE5NzAsIm5iZiI6MTU5MTk2ODM3MCwiYXVkIjoiM2ViOTk5YTktMzlhNi00ODJmLThjZTktZjU3MmM2NGJiNmYwIiwic3ViIjoiMmRjNDc5YjItMjQzYy00OGM2LTg3NmMtYTMyZjdkZDE2NmNiIiwiZ2l2ZW5fbmFtZSI6IlNoYXciLCJmYW1pbHlfbmFtZSI6IklubmVzIiwiZW1haWxzIjpbInNoYXdAaW1tb3J0YWwubmV0LmF1Il0sInRmcCI6IkIyQ18xX3NpZ251cF9zaWduaW4iLCJzY3AiOiJyZWFkIHVzZXJfaW1wZXJzb25hdGlvbiIsImF6cCI6IjFiOGZjMGJmLWFhZGItNDY0Yi1hZmY4LWRkYjU5OGE0OWY2ZCIsInZlciI6IjEuMCIsImlhdCI6MTU5MTk2ODM3MH0.hqTm93DRbcY_1zWfDwz0B64uuucfxp48v_4wtMurj2YOoN-THuLdlY23hPjqUf53cViuBAlucFvccibg96engx2Hbo1zJpSW11l8F6b5TXY8rX9kjnJG3qtjtqfoyLuQuNRu76-AVYYXVi5gkRk-j6KLFdTgoQ322cZnsDqMFAFW1psovRC0V75wJyepo65FMOIl9kgSBkP0P_0MxEP2ZCCAxwJaSGMYoFoPbkX6CHjNYg7AEAxJC0as7oKCcXNh8PLXgKmjDdX1PQcPNwiE2oiX6il-EQnbfqZjVoAVCqhbtRBghMTo0k1qTcnWHmPF4pL_PlxMuxu_VIA68KHkCg";

            ICloudScaleClient client = new CloudScaleClient(new CloudScaleClientConfiguration
                {BaseUrl = "https://localhost:5001"});

            var healthResponse = await client.GetHealth(bearerToken);
            Log.Information("Health Response {Response}", healthResponse);

            var pingResponse =
                await client.GetPing(bearerToken, new PingRequest {Message = Guid.NewGuid().ToString("N")});
            Log.Information("Ping Response {Response}", pingResponse.Message);

            var weatherResponse = await client.GetWeatherForecast(bearerToken, new WeatherRequest());
            Log.Information("Weather Response {@Response}", weatherResponse);

            Log.Information("Ending CLI");

            Log.CloseAndFlush();
        }
    }
}