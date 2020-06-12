using System.Threading.Tasks;
using CloudScale.Contracts.Ping;
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

        public CloudScaleClient(CloudScaleClientConfiguration configuration, JsonSerializerSettings jsonSerializerSettings)
        {
            _configuration = configuration;
            
            FlurlHttp.Configure(settings =>
            {
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSerializerSettings);
            });
        }

        public Task<string> GetHealth()
        {
            return _configuration.BaseUrl
                .AppendPathSegment("healthz")
                .GetStringAsync();
        }

        public Task<PingResponse> GetPing(PingRequest request)
        {
            return _configuration.BaseUrl
                .AppendPathSegment("ping")
                .PostJsonAsync(request)
                .ReceiveJson<PingResponse>();
        }

        public Task<WeatherResponse> GetWeatherForecast(WeatherRequest request)
        {
            return _configuration.BaseUrl
                .AppendPathSegment("weatherforecast") //TODO: Work out a way to remove these as "magic strings"
                .GetJsonAsync<WeatherResponse>();
        }
    }
}