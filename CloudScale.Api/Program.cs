using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using ILogger = Serilog.ILogger;

namespace CloudScale.Api
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
    
    public class Program
    {
        public static void Main(string[] args)
        {
            var assembly = Assembly.GetEntryAssembly() ?? throw new NullReferenceException();
            var assemblyName = assembly.GetName().Name;
            var assemblyVersion = assembly.GetName().Version;
            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true)
                .AddJsonFile("appsettings.development.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
            
            Log.Logger = new LoggingFactory(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // <- Add this line
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}