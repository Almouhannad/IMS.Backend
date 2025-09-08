using IMS.SharedKernel.Config;

namespace IMS.Config;

public static class CONFIG
{
    // Env-var names
    private const string SqlServerConnectionStringVar = "SQL_SERVER_CONNECTION_STRING";
    private const string CSVSeedPathVar = "CSV_SEED_PATH";
    private const string SeqServerUrlVar = "SEQ_SERVER_URL";

    // Lazy, exception-safe properties

    private static readonly Lazy<string> _sqlServerConnectionString = new(() => ENVHelper.GetEnv(SqlServerConnectionStringVar));
    public static string SqlServerConnectionString => _sqlServerConnectionString.Value;

    private static readonly Lazy<string> _csvSeedPath = new(() => ENVHelper.GetEnv(CSVSeedPathVar));
    public static string CSVSeedPath => _csvSeedPath.Value;

    private static readonly Lazy<Uri> _seqServerUrl = new(() => ENVHelper.ParseUri(SeqServerUrlVar));
    public static Uri SeqServerUrl => _seqServerUrl.Value;
}