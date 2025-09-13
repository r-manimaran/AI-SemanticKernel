using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi;

public class CheckSqlConnection : IHealthCheck
{
    private readonly string _connectionString;

    public CheckSqlConnection(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try 
        {             
            using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync();
            using var command = new SqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync();

            return HealthCheckResult.Healthy("SQL Database is reachable.");
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(status:context.Registration.FailureStatus, description:"SQL Database is not reachable.", exception:ex);
        }
    }
}
