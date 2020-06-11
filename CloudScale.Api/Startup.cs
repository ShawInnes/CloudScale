using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CloudScale.Core.Handlers.Ping;
using CloudScale.Data;
using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Serilog;
using SystemClock = NodaTime.SystemClock;

namespace CloudScale.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });

            var handlersAssembly = typeof(PingHandler).Assembly;
            var validatorsAssembly = typeof(PingRequestValidator).Assembly;
            
            services.AddMediatR(handlersAssembly);
            
            services.AddControllers()
                .AddFluentValidation(o => o.RegisterValidatorsFromAssembly(validatorsAssembly))
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

            services.AddTransient<IClock>(provider => SystemClock.Instance);

            services.AddSwaggerDocument();

            services.AddDbContext<CloudScaleDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BloggingDatabase"),
                    o =>
                    {
                        o.UseNodaTime();
                        o.EnableRetryOnFailure();
                    }));

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/liveness", options: new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}