using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using CybersecurityAwarenessBot.Models;
using MySqlConnector;

namespace CybersecurityAwarenessBot.Services
{
    public class TaskService
    {
        // MySQL is the primary store when configured; JSON is the safe fallback.
        private const string CreateTableSql = """
            CREATE TABLE IF NOT EXISTS cyber_tasks (
                id CHAR(36) NOT NULL PRIMARY KEY,
                owner_name VARCHAR(100) NOT NULL,
                title VARCHAR(255) NOT NULL,
                description TEXT NOT NULL,
                reminder_date DATETIME NULL,
                is_completed TINYINT(1) NOT NULL DEFAULT 0,
                created_at DATETIME NOT NULL
            );
            """;

        private readonly string _filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "cyber-tasks.json");
        private readonly string _connectionString = TaskDatabaseSettings.ConnectionString;
        private readonly string _ownerName;
        private bool _databaseAvailable;

        public ObservableCollection<CyberTask> Tasks { get; } = new();
        public bool UsesDatabase => _databaseAvailable;

        public TaskService(string ownerName)
        {
            _ownerName = ownerName.Trim();
            Load();
        }

        public CyberTask Add(string title, string description, DateTime? reminderDate)
        {
            var task = new CyberTask
            {
                OwnerName = _ownerName,
                Title = title.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? title.Trim() : description.Trim(),
                ReminderDate = reminderDate
            };

            if (_databaseAvailable && TryInsert(task))
            {
                Tasks.Add(task);
                return task;
            }

            Tasks.Add(task);
            SaveJson();
            return task;
        }

        public bool Complete(Guid id)
        {
            CyberTask task = Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return false;
            }

            if (_databaseAvailable && !TryUpdateCompletion(task))
            {
                return false;
            }

            task.IsCompleted = true;
            if (!_databaseAvailable)
            {
                SaveJson();
            }

            return true;
        }

        public bool UpdateReminder(Guid id, DateTime reminderDate)
        {
            CyberTask task = Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return false;
            }

            if (_databaseAvailable && !TryUpdateReminder(task.Id, reminderDate))
            {
                return false;
            }

            task.ReminderDate = reminderDate;
            if (!_databaseAvailable)
            {
                SaveJson();
            }

            return true;
        }

        public bool ClearAll()
        {
            if (_databaseAvailable && !TryClearAll())
            {
                return false;
            }

            Tasks.Clear();
            if (!_databaseAvailable)
            {
                SaveJson();
            }

            return true;
        }

        public bool Delete(Guid id)
        {
            CyberTask task = Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return false;
            }

            if (_databaseAvailable && !TryDelete(id))
            {
                return false;
            }

            Tasks.Remove(task);
            if (!_databaseAvailable)
            {
                SaveJson();
            }

            return true;
        }

        private void Load()
        {
            // Startup chooses MySQL only when a connection string is configured
            // and the required schema can be used successfully.
            if (TaskDatabaseSettings.IsConfigured && TryLoadFromDatabase())
            {
                _databaseAvailable = true;
                return;
            }

            LoadJson();
        }

        private bool TryLoadFromDatabase()
        {
            // This method is useful to show in the video: it creates a fresh table,
            // checks schema compatibility, and loads tasks for the current user.
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using (var createCommand = new MySqlCommand(CreateTableSql, connection))
                {
                    createCommand.ExecuteNonQuery();
                }

                using (var columnCheckCommand = new MySqlCommand(
                    """
                    SELECT COUNT(*)
                    FROM information_schema.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = 'cyber_tasks'
                      AND COLUMN_NAME = 'owner_name'
                    """,
                    connection))
                {
                    bool ownerColumnExists = Convert.ToInt32(columnCheckCommand.ExecuteScalar()) > 0;
                    if (!ownerColumnExists)
                    {
                        // The application user should not need ALTER permission. A newly
                        // created table already includes this column; an older incompatible
                        // schema uses the JSON fallback instead of failing at startup.
                        return false;
                    }
                }

                using var command = new MySqlCommand(
                    "SELECT id, owner_name, title, description, reminder_date, is_completed, created_at FROM cyber_tasks WHERE owner_name = @owner_name ORDER BY created_at",
                    connection);
                command.Parameters.AddWithValue("@owner_name", _ownerName);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Tasks.Add(new CyberTask
                    {
                        Id = Guid.Parse(reader.GetString("id")),
                        OwnerName = reader.GetString("owner_name"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date"))
                            ? null
                            : reader.GetDateTime("reminder_date"),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        CreatedAt = reader.GetDateTime("created_at")
                    });
                }

                return true;
            }
            catch
            {
                Tasks.Clear();
                return false;
            }
        }

        private bool TryUpdateReminder(Guid id, DateTime reminderDate)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(
                    "UPDATE cyber_tasks SET reminder_date = @reminder_date WHERE id = @id AND owner_name = @owner_name",
                    connection);
                command.Parameters.AddWithValue("@reminder_date", reminderDate);
                command.Parameters.AddWithValue("@id", id.ToString());
                command.Parameters.AddWithValue("@owner_name", _ownerName);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                _databaseAvailable = false;
                return false;
            }
        }

        private bool TryClearAll()
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(
                    "DELETE FROM cyber_tasks WHERE owner_name = @owner_name",
                    connection);
                command.Parameters.AddWithValue("@owner_name", _ownerName);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                _databaseAvailable = false;
                return false;
            }
        }

        private bool TryInsert(CyberTask task)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(
                    "INSERT INTO cyber_tasks (id, owner_name, title, description, reminder_date, is_completed, created_at) VALUES (@id, @owner_name, @title, @description, @reminder_date, @is_completed, @created_at)",
                    connection);
                AddParameters(command, task);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                _databaseAvailable = false;
                return false;
            }
        }

        private bool TryUpdateCompletion(CyberTask task)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(
                    "UPDATE cyber_tasks SET is_completed = 1 WHERE id = @id AND owner_name = @owner_name",
                    connection);
                command.Parameters.AddWithValue("@id", task.Id.ToString());
                command.Parameters.AddWithValue("@owner_name", _ownerName);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                _databaseAvailable = false;
                return false;
            }
        }

        private bool TryDelete(Guid id)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(
                    "DELETE FROM cyber_tasks WHERE id = @id AND owner_name = @owner_name",
                    connection);
                command.Parameters.AddWithValue("@id", id.ToString());
                command.Parameters.AddWithValue("@owner_name", _ownerName);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                _databaseAvailable = false;
                return false;
            }
        }

        private static void AddParameters(MySqlCommand command, CyberTask task)
        {
            command.Parameters.AddWithValue("@id", task.Id.ToString());
            command.Parameters.AddWithValue("@owner_name", task.OwnerName);
            command.Parameters.AddWithValue("@title", task.Title);
            command.Parameters.AddWithValue("@description", task.Description);
            command.Parameters.AddWithValue("@reminder_date", task.ReminderDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@is_completed", task.IsCompleted);
            command.Parameters.AddWithValue("@created_at", task.CreatedAt);
        }

        private void LoadJson()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return;
                }

                var tasks = JsonSerializer.Deserialize<List<CyberTask>>(File.ReadAllText(_filePath));
                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        if (string.Equals(task.OwnerName, _ownerName, StringComparison.OrdinalIgnoreCase))
                        {
                            Tasks.Add(task);
                        }
                    }
                }
            }
            catch
            {
                Tasks.Clear();
            }
        }

        private void SaveJson()
        {
            try
            {
                var allTasks = File.Exists(_filePath)
                    ? JsonSerializer.Deserialize<List<CyberTask>>(File.ReadAllText(_filePath)) ?? new List<CyberTask>()
                    : new List<CyberTask>();

                allTasks.RemoveAll(task => string.Equals(task.OwnerName, _ownerName, StringComparison.OrdinalIgnoreCase));
                allTasks.AddRange(Tasks);

                string directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_filePath, JsonSerializer.Serialize(allTasks));
            }
            catch
            {
                // Local persistence failures must not crash the GUI.
            }
        }
    }
}
