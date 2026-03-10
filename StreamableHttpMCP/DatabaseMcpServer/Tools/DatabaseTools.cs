using DatabaseMcpServer.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace DatabaseMcpServer.Tools;

[McpServerToolType]
public class DatabaseTools (DatabaseService db)
{
    //--1. Run SQL Query ------

    [McpServerTool, Description(
        "Execute a read-only SELECT query against the SQL server database."+
        "Only SELECT statements are permitted. Update, DELETE, INSERT and DDL are blocked.")]
    public async Task<string> QuerySqlServer(
        [Description("A valid SELECT SQL query")] string sql)
    {
        var result = await db.ExecuteQueryAsync(DatabaseType.SqlServer, sql);
        
        return result.IsSuccess ? result.FormattedData!
            : result.IsBlocked ? result.ErrorMessage!
            : result.ErrorMessage!;

    }

    [McpServerTool, Description(
        "Execute a read-only SELECT query against the SQLite database." +
        "Only SELECT Statements are permitted. UPDATE, DELETE, INSERT and DDL are blocked.")]
    public async Task<string> QuerySqlite([Description("A valid SELECT SQL Query")] string sql)
    {
        var result = await db.ExecuteQueryAsync(DatabaseType.Sqlite, sql);

        return result.IsSuccess? result.FormattedData!
            : result.IsBlocked ? result.ErrorMessage! 
            : result.ErrorMessage!;
    }

    // -- 2. Schema Discovery ------------
    [McpServerTool, Description(
        "List all tables and views in the SQL Server database.")]
    public async Task<string> ListSqlServerTables()
        => await db.GetTablesAsync(DatabaseType.SqlServer);

    [McpServerTool, Description(
        "List all tables and views in the SQLite database.")]
    public async Task<string> ListSqliteTables()
        => await db.GetTablesAsync(DatabaseType.Sqlite);

    [McpServerTool, Description(
        "Get column names, data types and nullability for a SQL Server table.")]
    public async Task<string> GetSqlServerTableSchema(
        [Description("Table name to inspect")] string tableName)
        => await db.GetColumnsAsync(DatabaseType.SqlServer, tableName);

    [McpServerTool, Description(
        "Get column names and data types for a SQLite table.")]
    public async Task<string> GetSqliteTableSchema(
        [Description("Table name to inspect")] string tableName)
        => await db.GetColumnsAsync(DatabaseType.Sqlite, tableName);
}
