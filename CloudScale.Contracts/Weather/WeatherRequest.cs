using MediatR;

namespace CloudScale.Contracts.Weather
{
    public class WeatherRequest : IRequest<WeatherResponse>
    {
        public WeatherRequest()
        {
            
        }
    }
}