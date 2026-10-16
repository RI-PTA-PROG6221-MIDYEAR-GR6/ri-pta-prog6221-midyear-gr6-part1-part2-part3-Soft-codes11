namespace CybersecurityAwarenessBot
{
    public class ChatMemory
    {
        // Stores arbitrary memory entries (e.g., interest: "privacy")
        public System.Collections.Generic.Dictionary<string, string> Data { get; } = new();

        // Last topic discussed (canonical token such as "password", "phish", etc.)
        public string? LastTopic { get; set; }

        private static string MemoryFilePath()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            string assets = System.IO.Path.Combine(dir, "Assets");
            if (!System.IO.Directory.Exists(assets))
            {
                System.IO.Directory.CreateDirectory(assets);
            }
            return System.IO.Path.Combine(assets, "memory.json");
        }

        public void Save()
        {
            try
            {
                var payload = new
                {
                    LastTopic = this.LastTopic,
                    Data = this.Data
                };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                System.IO.File.WriteAllText(MemoryFilePath(), json);
            }
            catch
            {
                // swallow save errors (non-fatal)
            }
        }

        public void Load()
        {
            try
            {
                var path = MemoryFilePath();
                if (!System.IO.File.Exists(path))
                {
                    return;
                }
                var json = System.IO.File.ReadAllText(path);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("LastTopic", out var lt))
                {
                    this.LastTopic = lt.GetString();
                }
                if (doc.RootElement.TryGetProperty("Data", out var data))
                {
                    foreach (var prop in data.EnumerateObject())
                    {
                        this.Data[prop.Name] = prop.Value.GetString() ?? string.Empty;
                    }
                }
            }
            catch
            {
                // ignore
            }
        }
    }
}
