namespace CybersecurityAwarenessBot.Services
{
    public static class TaskDatabaseSettings
    {
        // The connection string is supplied at runtime so credentials are not committed.
        public const string ConnectionStringEnvironmentVariable = "CYBERBOT_MYSQL_CONNECTION_STRING";

        public static string ConnectionString =>
            Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable) ?? string.Empty;

        public static bool IsConfigured => !string.IsNullOrWhiteSpace(ConnectionString);
    }
}
