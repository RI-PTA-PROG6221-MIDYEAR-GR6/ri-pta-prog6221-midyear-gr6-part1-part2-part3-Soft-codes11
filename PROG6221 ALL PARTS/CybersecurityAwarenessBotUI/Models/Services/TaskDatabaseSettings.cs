namespace CybersecurityAwarenessBot.Services
{
    public static class TaskDatabaseSettings
    {
        public const string ConnectionStringEnvironmentVariable = "CYBERBOT_MYSQL_CONNECTION_STRING";

        public static string ConnectionString =>
            Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable) ?? string.Empty;

        public static bool IsConfigured => !string.IsNullOrWhiteSpace(ConnectionString);
    }
}
