using System.Text.RegularExpressions;
using CybersecurityAwarenessBot.Data;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    /// <summary>
    /// Detects simple sentiments (worried, frustrated, confused, etc.) in a message
    /// and supplies an empathetic opening line for the bot's reply.
    /// </summary>
    public class SentimentAnalyzer
    {
        private readonly Dictionary<Sentiment, Regex> _patterns = new();

        public SentimentAnalyzer()
        {
            foreach (var entry in KnowledgeBase.SentimentKeywords)
            {
                string alternatives = string.Join("|", entry.Value.Select(Regex.Escape));

                // The lookbehinds ignore negated words, e.g. "not worried" or "never confused".
                _patterns[entry.Key] = new Regex(
                    $@"(?<!\bnot\s)(?<!\bnever\s)\b({alternatives})",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
        }

        /// <summary>Returns the first sentiment found, or null if the text is emotionally neutral.</summary>
        public Sentiment? Detect(string text)
        {
            foreach (var entry in _patterns)
            {
                if (entry.Value.IsMatch(text))
                {
                    return entry.Key;
                }
            }

            return null;
        }

        /// <summary>Picks a random empathetic opener for the sentiment, personalised with the name.</summary>
        public string GetOpener(Sentiment sentiment, string name)
        {
            string[] options = KnowledgeBase.SentimentOpeners[sentiment];
            return options[Random.Shared.Next(options.Length)].Replace("{name}", name);
        }
    }
}
