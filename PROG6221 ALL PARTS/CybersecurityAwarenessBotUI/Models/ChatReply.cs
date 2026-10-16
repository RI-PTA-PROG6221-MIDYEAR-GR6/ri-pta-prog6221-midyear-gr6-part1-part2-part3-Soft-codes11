namespace CybersecurityAwarenessBot.Models
{
    /// <summary>
    /// What the chat engine hands back to the GUI: the text to show and,
    /// optionally, the mood it detected in the user's message.
    /// </summary>
    public record ChatReply(string Text, Sentiment? Mood = null);
}
