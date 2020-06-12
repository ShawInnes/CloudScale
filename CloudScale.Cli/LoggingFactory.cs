using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Serilog;
using Serilog.Exceptions;

namespace CloudScale.Cli
{
    public class LoggingFactory
    {
        private readonly IConfiguration _configuration;
        
        public LoggingFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ILogger CreateLogger()
        {
            var assembly = Assembly.GetEntryAssembly() ?? throw new NullReferenceException();
            var assemblyName = assembly.GetName().Name;
            var assemblyVersion = assembly.GetName().Version;

            var loggerConfig = new LoggerConfiguration()
                .ConfigureForNodaTime(DateTimeZoneProviders.Tzdb)
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ApplicationName", assemblyName)
                .Enrich.WithProperty("ApplicationVersion", assemblyVersion)
                .WriteTo.Console()
                .WriteTo.Seq(
                    Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341");

            return loggerConfig.CreateLogger();
        }
    }
}