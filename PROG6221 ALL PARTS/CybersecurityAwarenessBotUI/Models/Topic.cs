using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot.Models
{
    /// <summary>
    /// A cybersecurity topic the bot knows about: keywords used to recognise it,
    /// several tips (used for random responses) and deeper explanations
    /// (used when the user asks "tell me more").
    /// </summary>
    public class Topic
    {
        private readonly Regex _pattern;

        public string Key { get; }
        public string DisplayName { get; }
        public string Intro { get; }
        public IReadOnlyList<string> Tips { get; }
        public IReadOnlyList<string> Details { get; }

        /// <summary>A clause that continues "As someone interested in X, ..." for memory recall.</summary>
        public string InterestNote { get; }

        public Topic(string key, string displayName, string[] keywords, string intro,
                     string[] tips, string[] details, string interestNote)
        {
            Key = key;
            DisplayName = displayName;
            Intro = intro;
            Tips = tips;
            Details = details;
            InterestNote = interestNote;

            // \b + keyword = word-start match, so "phish" also matches "phishing".
            string alternatives = string.Join("|", keywords.Select(Regex.Escape));
            _pattern = new Regex($@"\b({alternatives})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>How many keyword hits this topic has in the text (0 = not mentioned).</summary>
        public int MatchScore(string text) => _pattern.Matches(text).Count;
    }
}
