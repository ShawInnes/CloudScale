using Ardalis.SmartEnum;

namespace CloudScale.Core.Handlers.Weather
{
    public sealed class WeatherSummaryEnum : SmartEnum<WeatherSummaryEnum>
    {
        public static readonly WeatherSummaryEnum Freezing = new WeatherSummaryEnum("Freezing", 1);
        public static readonly WeatherSummaryEnum Bracing = new WeatherSummaryEnum("Bracing", 2);
        public static readonly WeatherSummaryEnum Chilly = new WeatherSummaryEnum("Chilly", 3);
        public static readonly WeatherSummaryEnum Cool = new WeatherSummaryEnum("Cool", 4);
        public static readonly WeatherSummaryEnum Mild = new WeatherSummaryEnum("Mild", 5);
        public static readonly WeatherSummaryEnum Warm = new WeatherSummaryEnum("Warm", 6);
        public static readonly WeatherSummaryEnum Balmy = new WeatherSummaryEnum("Balmy", 7);
        public static readonly WeatherSummaryEnum Hot = new WeatherSummaryEnum("Hot", 8);
        public static readonly WeatherSummaryEnum Sweltering = new WeatherSummaryEnum("Sweltering", 9);
        public static readonly WeatherSummaryEnum Scorching = new WeatherSummaryEnum("Scorching", 10);

        private WeatherSummaryEnum(string name, int value) : base(name, value)
        {
        }
    }
}