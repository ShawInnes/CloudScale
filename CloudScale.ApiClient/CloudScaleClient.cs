using System.Threading.Tasks;
using CloudScale.Contracts.Ping;
using CloudScale.Contracts.Recursive;
using CloudScale.Contracts.Weather;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace CloudScale.ApiClient
{
    public class CloudScaleClient : ICloudScaleClient
    {
        private readonly CloudScaleClientConfiguration _configuration;

        /*
            This may be required later
            FlurlHttp.Configure(settings =>
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                }.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSerializerSettings);
            });
         */
        public CloudScaleClient(CloudScaleClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> GetHealth(string token)
        {
            return _configuration.BaseUrl
                .AppendPathSegment("healthz")
                .WithOAuthBearerToken(token)
                .GetStringAsync();
        }

        public Task<PingResponse> GetPing(string token, PingRequest request)
        {
            return _configuration.BaseUrl
                .AppendPathSegment("ping")
                .WithOAuthBearerToken(token)
                .PostJsonAsync(request)
                .ReceiveJson<PingResponse>();
        }

        public Task<WeatherResponse> GetWeatherForecast(string token,WeatherRequest request)
        {
            return _configuration.BaseUrl
                .AppendPathSegment("weatherforecast") //TODO: Work out a way to remove these as "magic strings"
                .WithOAuthBearerToken(token)
                .GetJsonAsync<WeatherResponse>();
        }
        
        public Task<RecursiveResponse> GetRecursive(string token,RecursiveRequest request)
        {
            return _configuration.BaseUrl
                .AppendPathSegment("recursive")
                .WithOAuthBearerToken(token)
                .PostJsonAsync(request)
                .ReceiveJson<RecursiveResponse>();
        }
    }
}