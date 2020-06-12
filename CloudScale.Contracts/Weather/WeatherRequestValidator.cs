using FluentValidation;

namespace CloudScale.Contracts.Weather
{
    public class WeatherRequestValidator : AbstractValidator<WeatherRequest>
    {
        public WeatherRequestValidator()
        {
        }
    }
}