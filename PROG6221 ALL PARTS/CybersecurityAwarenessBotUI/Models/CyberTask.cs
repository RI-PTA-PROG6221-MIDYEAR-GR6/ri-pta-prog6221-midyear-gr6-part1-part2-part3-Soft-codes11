namespace CybersecurityAwarenessBot.Models
{
    public class CyberTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string OwnerName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string ReminderText => ReminderDate.HasValue
            ? ReminderDate.Value.ToString("dd MMM yyyy 'at' HH:mm")
            : "No reminder";

        public string StatusText => IsCompleted ? "Completed" : "Open";
    }
}
