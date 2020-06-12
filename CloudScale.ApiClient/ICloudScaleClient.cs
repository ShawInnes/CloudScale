using System.Threading.Tasks;
using CloudScale.Contracts.Ping;
using CloudScale.Contracts.Weather;

namespace CloudScale.ApiClient
{
    public interface ICloudScaleClient
    {
        Task<string> GetHealth();
        Task<PingResponse> GetPing(PingRequest request);
        Task<WeatherResponse> GetWeatherForecast(WeatherRequest request);
    }
}