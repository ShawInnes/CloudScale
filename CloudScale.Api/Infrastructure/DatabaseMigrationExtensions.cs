using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CloudScale.Api.Infrastructure
{
    public static class DatabaseMigrationExtensions
    {
        public static IApplicationBuilder MigrateDatabase<T>(this IApplicationBuilder app,
            IServiceScopeFactory scopeFactory) where T : DbContext
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<T>();
                db.Database
                    .EnsureCreated();

                var migrations = db.Database.GetPendingMigrations();
                if (migrations.Any())
                    db.Database.Migrate();
            }

            return app;
        }
    }
}