using FluentValidation;

namespace CloudScale.Core.Handlers.Weather
{
    public class WeatherRequestValidator : AbstractValidator<WeatherRequest>
    {
        public WeatherRequestValidator()
        {
        }
    }
}