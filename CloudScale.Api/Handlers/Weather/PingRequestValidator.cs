using FluentValidation;

namespace CloudScale.Api.Handlers.Weather
{
    public class WeatherRequestValidator : AbstractValidator<WeatherRequest>
    {
        public WeatherRequestValidator()
        {
        }
    }
}