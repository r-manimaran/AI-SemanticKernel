using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace SqlChat.Agent;

public class SqlQueryTool
{
    private readonly string _connectionString;
    private readonly ILogger<SqlQueryTool> _logger;

    public SqlQueryTool(IConfiguration configuration, ILogger<SqlQueryTool> logger)
    {
        _connectionString = configuration.GetConnectionString("SqlChatDatabase") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        _logger = logger;
    }
    [KernelFunction]
    public async Task<string> ExecuteQueryAsync(string sqlQuery)
    {
        try
        {
            _logger.LogInformation(sqlQuery);

            if (!sqlQuery.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                return "Only SELECT queries are allowed.";

            using var conn = new SqlConnection(_connectionString);

            var result = await conn.QueryAsync(sqlQuery);

            return JsonSerializer.Serialize(result);
                        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return $"Error executing query: {ex.Message}";
        }
    }
}
