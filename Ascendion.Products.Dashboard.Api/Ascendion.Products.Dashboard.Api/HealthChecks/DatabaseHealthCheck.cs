using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ascendion.Products.Dashboard.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=app.db");
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";
                command.ExecuteScalar();
                return HealthCheckResult.Healthy("SQLite is healthy");
            }
            catch (Exception ex) 
            {
                return HealthCheckResult.Unhealthy("SQLite is unhealthy", ex);
            }
        }
    }
}
