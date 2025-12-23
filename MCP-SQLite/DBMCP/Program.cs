using DBMCP;
using DBMCP.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

public class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Starting BookCraft MCP Server...");

     var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

      /*  Initial test
       *  
       var connectionString = $"Data Source={configuration["Database:Path"]}";

        var schemaService = new SqliteSchemaService(connectionString);
        
        
        Console.WriteLine("-----Tables-----");
        var tables = schemaService.GetTables().Take(2);
        var tablesYaml = tables.ToYaml();
        Console.WriteLine(tablesYaml);


        Console.WriteLine("-----Views-----");
        var views = schemaService.GetViews().Take(2);
        var viewsYaml = views.ToYaml();
        Console.WriteLine(viewsYaml);

        Console.WriteLine("---Specific Table Info : Loans ---");
        var loansTable = schemaService.GetTable("Loans");
        var loansTableYaml = loansTable?.ToYaml() ?? "Table 'Loans' not found.";
        Console.WriteLine(loansTableYaml);

        Console.WriteLine("---Foreign Key Relations ---");
        var foreignKeys = schemaService.GetForeignKeyRelations().Take(2);
        var foreignKeysYaml = foreignKeys.ToYaml();
        Console.WriteLine(foreignKeysYaml);
      */


        var builder = Host.CreateApplicationBuilder(settings: null);
        // Configure logging and add Console output with Trace level
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConsole(options =>
            {
                options.LogToStandardErrorThreshold = LogLevel.Trace;
            });
        });

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<Program>();
        
        BookCraftDbMcpServer.Configure(configuration, logger);
        
        // Test database connection
        try
        {
            var connectionString = $"Data Source={configuration["Database:Path"]}";
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
            var tableCount = (long)command.ExecuteScalar();
            
            logger.LogInformation("Database connection successful. Found {TableCount} tables.", tableCount);
            
            // List table names
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                logger.LogInformation("Table: {TableName}", reader.GetString(0));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database connection failed");
            return;
        }
        
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithToolsFromAssembly();

        await builder.Build().RunAsync();
    }
}

