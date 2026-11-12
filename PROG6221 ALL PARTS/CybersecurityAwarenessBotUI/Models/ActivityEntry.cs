namespace CybersecurityAwarenessBot.Models
{
    public class ActivityEntry
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;

        public string DisplayText => $"{Timestamp:dd MMM HH:mm} - {Description}";
    }
}
