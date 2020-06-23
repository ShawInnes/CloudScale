using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CloudScale.Api.Infrastructure.HealthChecks
{
    public class AliveHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            return Task.FromResult(HealthCheckResult.Healthy(data: new Dictionary<string, object>
                {{"Version", version}}
            ));
        }
    }
}