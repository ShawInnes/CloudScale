using MediatR;

namespace CloudScale.Api.Handlers.Weather
{
    public class WeatherRequest : IRequest<WeatherResponse>
    {
        public WeatherRequest()
        {
            
        }
    }
}