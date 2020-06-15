using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudScale.Contracts.Weather;
using MediatR;
using NodaTime;
using NodaTime.Extensions;

namespace CloudScale.Business.Weather
{
    public class WeatherHandler : IRequestHandler<WeatherRequest, WeatherResponse>
    {
        private readonly IClock _clock;

        public WeatherHandler(IClock clock)
        {
            _clock = clock;
        }

        public Task<WeatherResponse> Handle(WeatherRequest request, CancellationToken cancellationToken)
        {
            var rng = new Random();

            var list = new WeatherResponse();
            list.AddRange(Enumerable.Range(1, 5).Select(index => new WeatherResponse.WeatherForecast
                {
                    Date = _clock.InTzdbSystemDefaultZone().GetCurrentDate().PlusDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    SummaryEnum = WeatherSummaryEnum.List.ElementAt(rng.Next(WeatherSummaryEnum.List.Count)).Name
                })
                .ToList());

            return Task.FromResult(list);
        }
    }
}