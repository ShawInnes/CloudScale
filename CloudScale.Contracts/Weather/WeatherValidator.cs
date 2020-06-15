using FluentValidation;

namespace CloudScale.Contracts.Weather
{
    public class WeatherValidator : AbstractValidator<WeatherRequest>
    {
        public WeatherValidator()
        {
        }
    }
}