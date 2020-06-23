using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CloudScale.Api.Infrastructure;
using CloudScale.Api.Infrastructure.HealthChecks;
using CloudScale.Api.Middleware;
using CloudScale.ApiClient;
using CloudScale.Business;
using CloudScale.Business.Ping;
using CloudScale.Contracts.Ping;
using CloudScale.Core;
using CloudScale.Data;
using CloudScale.Data.Repositories;
using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Jaeger;
using Jaeger.Samplers;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using OpenTracing;
using OpenTracing.Contrib.NetCore.CoreFx;
using OpenTracing.Util;
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
            var validatorsAssembly = typeof(PingValidator).Assembly;

            services.AddMediatR(handlersAssembly);

            services.AddHttpContextAccessor();
            services.AddTransient<IBearerAccessor, BearerAccessor>();
            services.AddTransient<IUserAccessor, UserAccessor>();

            services.AddTransient(typeof(IRequestPostProcessor<,>), typeof(PaginationPipeline<,>));

            services.AddTransient<ICloudScaleRepository, CloudScaleRepository>();

            services.AddControllers()
                .AddFluentValidation(o => o.RegisterValidatorsFromAssembly(validatorsAssembly))
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));

            services.AddResponseCaching();

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

            var sqlConnectionString = Configuration.GetConnectionString("CloudScale");
            services.AddDbContext<CloudScaleDbContext>(options =>
            {
                options.UseSqlServer(sqlConnectionString,
                    o =>
                    {
                        o.UseNodaTime();
                        o.EnableRetryOnFailure();
                    });
            });

            services.AddSingleton(Log.Logger);

            Uri jaegerUri = new Uri("http://localhost:14268/api/traces");

            services.AddSingleton(serviceProvider =>
            {
                string serviceName = Assembly.GetEntryAssembly()?.GetName().Name;

                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ISampler sampler = new ConstSampler(sample: true);

                ITracer tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });

            // Prevent endless loops when OpenTracing is tracking HTTP requests to Jaeger.
            services.Configure<HttpHandlerDiagnosticOptions>(options =>
            {
                options.IgnorePatterns.Add(request => jaegerUri.IsBaseOf(request.RequestUri));
            });

            services.AddOpenTracing();

            services
                .AddHealthChecks()
                .AddCheck<AliveHealthCheck>("self", HealthStatus.Unhealthy)
                .AddSqlServer(sqlConnectionString, name: "database", tags: new[] {"database"});

            services
                .AddHealthChecksUI(setupSettings: setup =>
                {
                    setup.AddHealthCheckEndpoint("self", "/healthz");
                })
                .AddInMemoryStorage();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceScopeFactory scopeFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(60)
                    };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] {"Accept-Encoding"};

                await next();
            });

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
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();
            });
            
            // app.UseHealthChecksUI(config=> config.UIPath = "/hc-ui");
            
            app.MigrateDatabase<CloudScaleDbContext>(scopeFactory);
        }
    }
}