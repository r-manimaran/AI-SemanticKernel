using System.Text;

namespace DatabaseMcpServer;

public enum DatabaseType
{ 
    SqlServer,
    Sqlite
}

public class QueryResult
{
    public bool IsSuccess { get; private set; }
    public bool IsBlocked { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? FormattedData { get; private set; }
    public int RowCount { get; private set; }

    public static QueryResult Success(
       List<IDictionary<string, object?>> rows, int maxRows)
    {
        var sb = new StringBuilder();
        if (!rows.Any())
        {
            sb.AppendLine("Query returned no results.");
            return new QueryResult { IsSuccess = true, FormattedData = sb.ToString() };
        }

        // Header
        var columns = rows.First().Keys.ToList();
        var colWidths = columns.Select(c =>
            Math.Max(c.Length,
                rows.Max(r => r[c]?.ToString()?.Length ?? 0))).ToList();

        sb.AppendLine(string.Join(" | ",
            columns.Select((c, i) => c.PadRight(colWidths[i]))));
        sb.AppendLine(string.Join("-+-",
            colWidths.Select(w => new string('-', w))));

        foreach (var row in rows)
            sb.AppendLine(string.Join(" | ",
                columns.Select((c, i) =>
                    (row[c]?.ToString() ?? "NULL").PadRight(colWidths[i]))));

        if (rows.Count >= maxRows)
            sb.AppendLine($"\n⚠️ Results capped at {maxRows} rows.");

        return new QueryResult
        {
            IsSuccess = true,
            FormattedData = sb.ToString(),
            RowCount = rows.Count
        };
    }
    public static QueryResult Blocked(string reason) =>
     new() { IsBlocked = true, ErrorMessage = $"🚫 Blocked: {reason}" };

    public static QueryResult Error(string message) =>
        new() { ErrorMessage = $"❌ Error: {message}" };
}

