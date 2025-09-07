using IMS.SharedKernel.Config;

namespace IMS.Config;

public static class CONFIG
{
    // Env-var names
    private const string SqlServerConnectionStringVar = "SQL_SERVER_CONNECTION_STRING";

    // Lazy, exception-safe properties

    private static readonly Lazy<string> _sqlServerConnectionString = new(() => ENVHelper.GetEnv(SqlServerConnectionStringVar));
    public static string SqlServerConnectionString => _sqlServerConnectionString.Value;
}