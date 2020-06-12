using System.Collections.Generic;
using System.Linq;
using CloudScale.ApiClient;
using CloudScale.Business;
using CloudScale.Contracts.Ping;
using CloudScale.Data;
using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
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


        // TODO: Configure B2C
        // TODO: Add Examples of Custom Authorization Pipeline Auth/AuthZ

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            
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
            services.AddTransient<ICloudScaleClient>(provider =>
                new CloudScaleClient(new CloudScaleClientConfiguration {BaseUrl = "https://localhost:5001"}));

            services.AddOpenApiDocument(document =>
            {
                document.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Description = "B2C authentication",
                    Flow = OpenApiOAuth2Flow.Implicit,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            Scopes = new Dictionary<string, string>
                            {
                                {
                                    "https://cloudscaledemo.onmicrosoft.com/cloudscale-api/user_impersonation",
                                    "Access the api as the signed-in user"
                                },
                                {
                                    "https://cloudscaledemo.onmicrosoft.com/cloudscale-api/read",
                                    "Read access to the API"
                                },
                            },
                            AuthorizationUrl =
                                "https://cloudscaledemo.b2clogin.com/cloudscaledemo.onmicrosoft.com/oauth2/v2.0/authorize?p=B2C_1_signup_signin",
                            TokenUrl =
                                "https://cloudscaledemo.b2clogin.com/cloudscaledemo.onmicrosoft.com/oauth2/v2.0/token?p=B2C_1_signup_signin"
                        },
                    }
                });

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
            });

            services.AddDbContext<CloudScaleDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CloudScaleDatabase"),
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
            app.UseSwaggerUi3(settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = "1b8fc0bf-aadb-464b-aff8-ddb598a49f6d",
                    AppName = "swagger-ui-client"
                };
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/healthz", options: new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}