using System.Text.RegularExpressions;

namespace DatabaseMcpServer.Services;

public class QueryGuardrailService
{
    // Dangerours keywords that must never appear in a query
    private static readonly HashSet<string> _blockedKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        // DML
        "INSERT", "UPDATE","DELETE","MERGE","UPSERT","REPLACE",

        // DDL
        "CREATE","ALTER","DROP","TRUNCATE","RENAME",

        // DCL
        "GRANT","REVOKE","DENY",

        // Execution
        "EXEC","EXECUTE","SP_","XP_",

        // Transactions (revent bypassing)
        "COMMIT","ROLLBACK","BEGIN TRAN",

        // Dangerous SQL server features
        "OPENROWSET","OPENDATASOURCE","BULK INSERT","SHUTDOWN","DBCC"
    };

    public GuardrailResult Validate(string sql)
    {
        if (string.IsNullOrEmpty(sql))
            return GuardrailResult.Fail("Query cannot be empty.");

        // Normalize - remove comments and extra whitespace
        var normalized = RemoveComments(sql).Trim();

        // Must start with SELECT
        if (!normalized.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            return GuardrailResult.Fail(
                "Only SELECT queries are allowed. Query must start with SELECT.");

        // Tokenize and check each word
        var tokens = normalized.Split(
            [' ', '\n', '\r','\t','(',')',';',','], StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            // Strip brackets e.f [Update] trick
            var clean = token.Trim('[', ']', '"', '\'', '`');

            if (_blockedKeywords.Contains(clean))
                return GuardrailResult.Fail(
                    $"Query contains forbidden keyword '{clean}'. Only SELECT statment are permitted.");
        }
            // Block multiple statements (semicolon injection)
            var statements = normalized
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
            if (statements.Count > 1)
                return GuardrailResult.Fail(
                    "Multiple statements are not allowed. Submit one SELECT query at a time");

        return GuardrailResult.Pass();
    }

    private static string RemoveComments(string sql)
    {
        // Remove --single line comments
        var noLineComments = Regex.Replace(sql, @"--[^\r\n]*", " ");

        // Remove /* block comments */
        var noBlockComments = Regex.Replace(noLineComments, @"/\*.*?\*/", " ",
            RegexOptions.Singleline);

        return noBlockComments;
    }

    public record GuardrailResult(bool IsValid, string? ErrorMessage)
    {
        public static GuardrailResult Pass() => new(true,null);
        public static GuardrailResult Fail(string reason) => new(false, reason);

    }
}
