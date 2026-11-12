using System.Collections.ObjectModel;
using System.Text.Json;
using System.IO;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class TaskService
    {
        private readonly string _filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "cyber-tasks.json");

        public ObservableCollection<CyberTask> Tasks { get; } = new();

        public TaskService()
        {
            Load();
        }

        public CyberTask Add(string title, string description, DateTime? reminderDate)
        {
            var task = new CyberTask
            {
                Title = title.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? title.Trim() : description.Trim(),
                ReminderDate = reminderDate
            };
            Tasks.Add(task);
            Save();
            return task;
        }

        public bool Complete(Guid id)
        {
            CyberTask task = Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return false;
            }

            task.IsCompleted = true;
            Save();
            return true;
        }

        public bool Delete(Guid id)
        {
            CyberTask task = Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return false;
            }

            Tasks.Remove(task);
            Save();
            return true;
        }

        private void Load()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var tasks = JsonSerializer.Deserialize<List<CyberTask>>(File.ReadAllText(_filePath));
                    if (tasks != null)
                    {
                        foreach (var task in tasks)
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

        private void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_filePath, JsonSerializer.Serialize(Tasks));
            }
            catch
            {
                // Local persistence failures must not crash the GUI.
            }
        }
    }
}
