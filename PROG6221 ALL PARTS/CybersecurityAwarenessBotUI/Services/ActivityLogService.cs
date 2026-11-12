using System.Text.Json;
using System.Collections.ObjectModel;
using System.IO;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class ActivityLogService
    {
        private const int MaximumEntries = 100;
        private readonly string _filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "activity-log.json");

        public ObservableCollection<ActivityEntry> Entries { get; } = new();

        public ActivityLogService()
        {
            Load();
        }

        public void Record(string description)
        {
            Entries.Add(new ActivityEntry { Description = description });
            while (Entries.Count > MaximumEntries)
            {
                Entries.RemoveAt(0);
            }

            Save();
        }

        public IReadOnlyList<ActivityEntry> Recent(int count = 10) =>
            Entries.Reverse().Take(count).ToList();

        private void Load()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var entries = JsonSerializer.Deserialize<List<ActivityEntry>>(File.ReadAllText(_filePath));
                    if (entries != null)
                    {
                        foreach (var entry in entries)
                        {
                            Entries.Add(entry);
                        }
                    }
                }
            }
            catch
            {
                Entries.Clear();
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

                File.WriteAllText(_filePath, JsonSerializer.Serialize(Entries));
            }
            catch
            {
                // Activity logging must never prevent the chatbot from running.
            }
        }
    }
}
