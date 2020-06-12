using System.Threading.Tasks;
using CloudScale.Contracts.Ping;
using CloudScale.Contracts.Recursive;
using CloudScale.Contracts.Weather;

namespace CloudScale.ApiClient
{
    public interface ICloudScaleClient
    {
        /// <summary>
        /// Get API Health Status
        /// </summary>
        /// <returns></returns>
        Task<string> GetHealth(string token);

        /// <summary>
        /// Perform a Ping
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Pong</returns>
        Task<PingResponse> GetPing(string token, PingRequest request);

        /// <summary>
        /// Get a 7 day weather forecast, just like the real weather, this is generated randomly and is inaccurate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<WeatherResponse> GetWeatherForecast(string token, WeatherRequest request);

        Task<RecursiveResponse> GetRecursive(string token, RecursiveRequest request);
    }
}