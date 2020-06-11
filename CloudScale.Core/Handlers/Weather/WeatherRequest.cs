using MediatR;

namespace CloudScale.Core.Handlers.Weather
{
    public class WeatherRequest : IRequest<WeatherResponse>
    {
        public WeatherRequest()
        {
            
        }
    }
}