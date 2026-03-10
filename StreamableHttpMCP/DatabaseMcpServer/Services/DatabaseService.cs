using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;


namespace DatabaseMcpServer.Services;

public class DatabaseService(IOptions<DatabaseToolsSettings> options, QueryGuardrailService guardrail)
{
    private readonly DatabaseToolsSettings _settings = options.Value;

    // Execute Query ------------
    public async Task<QueryResult> ExecuteQueryAsync(DatabaseType dbType, string sql)
    {
        // 1. Guardrail check First - before touching the DB
        var validation = guardrail.Validate(sql);
        if (!validation.IsValid)
            return QueryResult.Blocked(validation.ErrorMessage);

        // 2. Get correct connection
        var dbSettings = dbType == DatabaseType.SqlServer ? _settings.SqlServer : _settings.Sqlite;

        if (!dbSettings.Enabled)
            return QueryResult.Blocked($"{dbType} is not enabled.");

        try
        {
            using var connection = CreateConnection(dbType, dbSettings.ConnectionString);
            await ((DbConnection)connection).OpenAsync();

            // 3. Execute with timeout + row limit enforced via DAPPER
            var limitedSql = ApplyRowLimit(sql, dbType, _settings.Guardrails.MaxRowsReturned);

            var rows = await connection.QueryAsync(limitedSql,
                        commandTimeout: _settings.Guardrails.QueryTimeoutSeconds);

            var resultList = rows.Cast<IDictionary<string, object>>().ToList();
            return QueryResult.Success(resultList, _settings.Guardrails.MaxRowsReturned);
        }
        catch (Exception ex)
        {
            return QueryResult.Error($"Query failed: {ex.Message}");
        }
    }

    // -- Schema Discovery ----------
    public async Task<string> GetTablesAsync(DatabaseType dbType)
    {
        var sql = dbType == DatabaseType.SqlServer
            ? "SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE FROM INFORMATION_SCHEMA.TABLES"
            : "SELECT name AS TABLE_NAME, type AS TABLE_TYPE FROM sqlite_master WHERE type IN ('table','view') ORDER BY name";

        var result = await ExecuteQueryAsync(dbType, sql);
        return result.IsSuccess ? result.FormattedData!: result.ErrorMessage!;
    }

    public async Task<string> GetColumnsAsync(DatabaseType dbType, string tableName)
    {
        // Sanitize table name - only allow alphanumeric, underscore and dot
        if (!Regex.IsMatch(tableName, @"^[\w\.\[\]]+$"))
            return "Invalid table name.";
        var sql = dbType == DatabaseType.SqlServer
            ? $"""
                SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = '{tableName}'
                ORDER BY ORDINAL_POSITION 
            """
                : $"PRAGMA table_info({tableName})";
        var result = await ExecuteQueryAsync(dbType,sql);
        return result.IsSuccess ? result.FormattedData! : result.ErrorMessage!;
    }

    // ---- Helpers
    private static IDbConnection CreateConnection(DatabaseType dbType, string connectionString)
    {
        return dbType == DatabaseType.SqlServer
            ? new SqlConnection(connectionString)
            : new SqliteConnection(connectionString);
    }

    private static string ApplyRowLimit(string sql, DatabaseType dbType, int maxRows)
    {
        // wrap in a subquery with row limit as extra safety net
        return dbType == DatabaseType.SqlServer
            ? $"SELECT TOP {maxRows} * FROM ({sql}) AS __mcp_query"
            : $"SELECT * FROM ({sql}) AS __mcp_query LIMIT {maxRows}";
    }
}



