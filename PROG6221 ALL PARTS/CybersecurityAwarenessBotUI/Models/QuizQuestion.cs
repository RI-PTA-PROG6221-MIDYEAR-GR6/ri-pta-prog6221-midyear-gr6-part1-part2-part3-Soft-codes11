namespace CybersecurityAwarenessBot.Models
{
    public class QuizQuestion
    {
        public string Question { get; init; } = string.Empty;
        public IReadOnlyList<string> Options { get; init; } = Array.Empty<string>();
        public int CorrectIndex { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public bool IsTrueFalse { get; init; }
    }
}
