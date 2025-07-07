using Ascendion.Products.Dashboard.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ascendion.Products.Dashboard.Auth;

public static class HealthCheckExtension
{
    public static void AddHealthCheckServices(this IServiceCollection services)
    {
        //services.AddHealthChecks()
        //    .AddCheck<DatabaseHealthCheck>("custom-sqlite-checker",HealthStatus.Unhealthy);

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("custom-sqlite-check",HealthStatus.Unhealthy)
            .AddSqlite("Data Source=app.db");

        services
            .AddHealthChecksUI(setupSettings:setup =>
            {
                setup.SetEvaluationTimeInSeconds(5);
            })
            .AddSqliteStorage($"Data Source=app.db");
    }
}
