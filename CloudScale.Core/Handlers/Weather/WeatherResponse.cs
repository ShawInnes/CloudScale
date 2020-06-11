using System.Collections.Generic;
using NodaTime;

namespace CloudScale.Core.Handlers.Weather
{
    public class WeatherResponse: List<WeatherResponse.WeatherForecast>
    {
        public class WeatherForecast
        {
            public LocalDate Date { get; set; }
            public int TemperatureC { get; set; }
            public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
            public string SummaryEnum { get; set; }
        }
    }
}