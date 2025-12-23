using DBMCP.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMCP;
[McpServerToolType,
 Description("""
    BookCraftDbMcpServer is a specialized MCP server tool for managing a SQLite database.
    It provides a structured and semantic way to 
    - Retrive full database schema information,
    - Retrieve information about tables, views, and relations,
    - Add new columns to existing tables,
    - Create new tables with specified columns.

    All operations are tailored for SQLite databases and utilize the Model Context Protocol (MCP) for communication.
    Any request outside the scope of these operations will be graciously handled with an error message or with humorous SQLite jokes.

    Use this server when you want to:
    - Write SQL queries for BookCraft database management,
    - Understand how the database is structured
    - Add or change schema elements like tables and columns,
    """)]
public static class BookCraftDbMcpServer
{
    private static string? _dbPath;
    private static ILogger? _logger;

    public static void Configure(IConfiguration configuration, ILogger? logger = null)
    {
        _dbPath = configuration["Database:Path"] ?? @"C:\Maran\Study\Git\Dotnet\AI-SemanticKernel\MCP-SQLite\DBMCP\bookcraft.db";
        _logger = logger;
        _logger?.LogInformation("BookCraftDbMcpServer configured with database path: {DbPath}", _dbPath);
    }

    private static string GetConnectionString()
    {
        if (string.IsNullOrWhiteSpace(_dbPath))
            throw new InvalidOperationException("Database path not configured.");
        _logger?.LogDebug("Creating connection string for database: {DbPath}", _dbPath);
        return $"Data Source={_dbPath}";
    }

    private static SqliteSchemaService CreateSqliteSchemaService() => new(GetConnectionString());
    [McpServerTool, Description("""
    Returns a complete general information about the BookCraft database, including the number of tables, views, and relations.
    Useful when a user asks: "what is BookCraftDb?" or "Tell me about BookCraftDb" or "what is the schema of BookCraftDb?
    """)]


    public static string GeneralInfo()
    {
        return """
            BookCraftDb is a SQLite database used by the BookCraft application.
            It contains various tables, views, and relations that store information about books, authors, and other related entities.
            The database schema is designed to efficiently manage and query this information.
            The database includes:
            - Tables: These are the primary data structures that store information.
            - Views: These are virtual tables that provide a specific representation of the data.
            - Relations: These define how tables are related to each other, typically through foreign keys.
            You can use the provided tools to explore the database schema, retrieve information about tables and views, and manage the database structure.
            """;
     }
    [McpServerTool,
     Description("""
        Returns the schema and metadata of all user-defined tables in the BookCraft database.
        This includes colmn info, primary keys, foreign keys, indexes, and triggers per table.

        Always use this tool to explore table structure before performing any operations on the database.
        If the user asks "What tables are in BookCraftDb?" or "Tell me about the tables in BookCraftDb?" or "What is the schema of BookCraftDb?" or
        "List tables",or "show schema", call this.
        """)]
    public static string GetTablesInfo()
    {
        _logger?.LogInformation("Retrieving tables information");
        var explorer = CreateSqliteSchemaService();
        return explorer.GetTables().ToYaml();
    }


    [McpServerTool, Description("""
        
        Returns detailed information about a specific table in the BookCraft database. This includes column information, primary keys, foreign keys, indexes, and triggers.
        Use this tool when you need to understand the structure of a specific table, such as when a user asks "What is the schema of the 'Books' table?" or "Tell me about the 'Authors' table?".
    
        If the table is not found, inform the user gracefully.
        
        """)]
    public static string GetTableInfo(string tableName)
    {
        var explorer = CreateSqliteSchemaService();
        var table = explorer.GetTable(tableName);
        return table != null ? table.ToYaml() : $"Table '{tableName}' not found in the database.";
    }

    [McpServerTool, Description("""
        Returns all foreign key relations between tables in the BookCraft database.
        This tool shows how tables are related to each other through foreign keys, which is essential for understanding the database structure.
        Use this tool when a user asks 
        - "What are the relations between tables in BookCraftDb?" 
        - "Tell me about the foreign keys in BookCraftDb?".
        - "Show table relations" 
        - "List relations" 
        - "Show foreign keys" 
        - "List foreign keys" 
        - "What are the relations in BookCraftDb?
        """)]    
    public static string GetTableRelationsInfo()
    {
        var explorer = CreateSqliteSchemaService();
        return explorer.GetForeignKeyRelations().ToYaml();
    }
    [McpServerTool, Description("""
    Return Metadata of all views in the BookCraft database, including their definitions and the tables they reference.
    Use this tool when a user asks 
    - "What views are in BookCraftDb?" 
    - "Tell me about the views in BookCraftDb?" 
    - "What is the schema of BookCraftDb views?".
    - "List views"
    - "What data is pre-aggregated in BookCraftDb?"

    Output is in YAML format with view names and SQL bodies for easy readability.
    """)]
    public static string GetViewsInfo()
    {
        var explorer = CreateSqliteSchemaService();
        return explorer.GetViews().ToYaml();
    }


    [McpServerTool,
     Description("""
        Adds a new column to an existing table in the BookCraft database.
        Requires:
        - tableName: The name of the table to which the column will be added.
        - columnName: The name of the new column to be added.
        - columnType: The data type of the new column (e.g., 'TEXT', 'INTEGER', 'REAL').

        If an invalid table name, column name, or column type is provided, convert it to the closest valid SQlite type.
        If there is an error (eg. duplicate column or bad name), return a friendly error message.

        Example usage:
        AddNewColumn("Books", "PublishedYear", "INTEGER");
    """)]
    public static string AddNewColumn(string tableName, string columnName, string columnType)
    {
        _logger?.LogInformation("Adding column {ColumnName} of type {ColumnType} to table {TableName}", columnName, columnType, tableName);
        var explorer = CreateSqliteSchemaService();
        return explorer.AddTableColumn(tableName, columnName, columnType);
    }


    [McpServerTool,
     Description("""
        Creates a new table with the specified name and columns.
        
        Columns should be provided as a dictionary where the key is the column name and the value is the column type.
        Input: Table name and a dictionary of columns. 
        Example:
        {   "Books": {
                "Title": "TEXT",
                "AuthorId": "INTEGER",
                "PublishedYear": "INTEGER NOT NULL"
            }
        }

        Ensure 
        - Column types are valid SQLite types (e.g., 'TEXT', 'INTEGER', 'REAL').
        - Inform the user if the table already exists or if there is an error creating the table.
        - If the table is created successfully, return a success message.
        """)]
    public static string CreateNewTable(string tableName, Dictionary<string, string> columns)
    {
        _logger?.LogInformation("Creating new table {TableName} with {ColumnCount} columns", tableName, columns.Count);
        var explorer = CreateSqliteSchemaService();
        return explorer.CreateTable(tableName, columns);
    }
}
