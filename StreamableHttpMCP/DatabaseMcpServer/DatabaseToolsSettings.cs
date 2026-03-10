namespace DatabaseMcpServer;

public class DatabaseToolsSettings
{
    public DbSettings SqlServer { get; set; } = new();
    public DbSettings Sqlite { get; set; } = new();
    public GuardrailSettings Guardrails { get; set; } = new();
}

public class DbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool Enabled { get; set; } = false;
}

public class GuardrailSettings
{
    public int MaxRowsReturned { get; set; } = 500;
    public int QueryTimeoutSeconds { get; set; } = 30;
}
