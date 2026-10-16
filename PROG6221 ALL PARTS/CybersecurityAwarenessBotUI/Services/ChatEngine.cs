using System.Globalization;
using System.Text.RegularExpressions;
using CybersecurityAwarenessBot.Data;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    /// <summary>
    /// A delegate describing one "intent handler". Each handler looks at the user's
    /// message and either returns a reply (it handled it) or null (pass it on).
    /// </summary>
    /// <param name="rawInput">The message exactly as typed (used for names).</param>
    /// <param name="text">The lower-cased, normalised message (used for matching).</param>
    public delegate ChatReply IntentHandler(string rawInput, string text);

    /// <summary>
    /// The brain of the chatbot. Messages travel through a pipeline of IntentHandler
    /// delegates (memory, recall, small talk, topics/sentiment/follow-ups) until one
    /// of them produces a reply; otherwise a friendly fallback is returned.
    /// </summary>
    public class ChatEngine
    {
        // ---------- Patterns ----------
        private static readonly Regex NamePattern = new(
            @"\b(?:my name is|call me|you can call me|i am called)\s+(?<name>[A-Za-z][A-Za-z'\-]{0,24})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex InterestPattern = new(
            @"\b(?:(?:i'?m|i am)\s+(?:really\s+|very\s+|quite\s+)?(?:interested in|into|curious about)|my fav(?:ou?rite)?\s+(?:cyber\s?security\s+)?topic is|i (?:like|love|enjoy) (?:learning|reading|hearing|talking) about)\s+(?<topic>.+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RecallPattern = new(
            @"what do you (?:remember|know) about me|do you remember|what'?s my name|what is my name|who am i|what am i interested in|what'?s my fav|what is my fav",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex FollowUpPattern = new(
            @"\b(another|more|explain|elaborate|continue|go on|details?|again|next|what do you mean|(?:don't|do not|dont) (?:understand|get it)|confus\w*|unclear|clarify|simpler|example)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DetailPattern = new(
            @"\b(more|explain|elaborate|details?|what do you mean|(?:don't|do not|dont) (?:understand|get it)|confus\w*|unclear|clarify|simpler|example|why)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex TipPattern = new(
            @"\b(tips?|advice|suggestions?)\b",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>Small-talk rules: pattern -> possible replies ({name} is replaced).</summary>
        private static readonly List<(Regex Pattern, string[] Replies)> SmallTalk = new()
        {
            (new Regex(@"^(bye|goodbye|exit|quit|see you|cya)\b", RegexOptions.IgnoreCase),
                new[] { "Thanks for chatting with me, {name}. Stay safe online! You can close the window whenever you're ready, or keep chatting." }),

            (new Regex(@"^(hi|hello|hey|howzit|sawubona|good (morning|afternoon|evening))( there| bot| cyberbot)?\W*$", RegexOptions.IgnoreCase),
                new[] { "Hello, {name}! Ready to learn something new about staying safe online?",
                        "Hi {name}! Ask me about passwords, phishing, privacy or anything else on your mind." }),

            (new Regex(@"how are you|how'?s it going|how do you feel", RegexOptions.IgnoreCase),
                new[] { "I'm doing great, {name}! I'm ready to help you learn more about staying safe online.",
                        "All systems secure and running smoothly! How can I help you today, {name}?" }),

            (new Regex(@"\bpurpose\b|what do you do|who are you|what are you|what is your job", RegexOptions.IgnoreCase),
                new[] { "My purpose is to share cybersecurity awareness tips and help you build safer online habits, {name}." }),

            (new Regex(@"what can i ask|what (topics|things) (can|do) you|^help$|what do you know|your topics", RegexOptions.IgnoreCase),
                new[] { "You can ask me about password safety, phishing and scams, privacy, safe browsing, malware, social engineering, account security and software updates. Try 'give me a phishing tip', tell me what you're interested in, or just tell me how you're feeling." }),

            (new Regex(@"\bthank|\bcheers\b|\bappreciate", RegexOptions.IgnoreCase),
                new[] { "You're welcome, {name}! Glad I could help.",
                        "Anytime, {name}! Ask me for another tip whenever you like." })
        };

        // ---------- State ----------
        private readonly User _user;
        private readonly SentimentAnalyzer _sentiment = new();
        private readonly List<IntentHandler> _pipeline;

        /// <summary>Shuffled "decks" so random tips don't repeat until the whole list has been used.</summary>
        private readonly Dictionary<string, Queue<int>> _decks = new();

        private Topic _currentTopic;
        private int _replyCount;

        /// <summary>Raised whenever the bot learns or updates something about the user.</summary>
        public event Action<User> MemoryChanged;

        public ChatEngine(User user)
        {
            _user = user;

            // The order matters: the first handler to return a reply wins.
            _pipeline = new List<IntentHandler>
            {
                HandleMemoryStatements,
                HandleRecall,
                HandleSmallTalk,
                HandleTopicConversation
            };
        }

        /// <summary>Main entry point: turns a user message into a reply. Never throws for odd input.</summary>
        public ChatReply GetReply(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ChatReply("I didn't catch that. Please type a question and I'll do my best to help.");
            }

            string raw = input.Trim();
            string text = Normalise(raw);

            foreach (IntentHandler handler in _pipeline)
            {
                ChatReply reply = handler(raw, text);
                if (reply != null)
                {
                    return reply;
                }
            }

            return Fallback();
        }

        // ---------- Handlers ----------

        /// <summary>Learns the user's name and favourite topic ("I'm interested in privacy").</summary>
        private ChatReply HandleMemoryStatements(string raw, string text)
        {
            Match nameMatch = NamePattern.Match(raw);
            if (nameMatch.Success)
            {
                _user.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameMatch.Groups["name"].Value.ToLowerInvariant());
                NotifyMemoryChanged();
                return new ChatReply($"Lovely to meet you, {_user.Name}! I'll remember your name for the rest of our chat.");
            }

            Match interestMatch = InterestPattern.Match(text);
            if (!interestMatch.Success)
            {
                return null;
            }

            string phrase = interestMatch.Groups["topic"].Value.Trim(' ', '.', '!', '?');
            Topic topic = FindTopic(phrase);

            if (topic == null)
            {
                _user.FavouriteTopic = phrase;
                NotifyMemoryChanged();
                return new ChatReply($"Interesting! I'll remember that you're interested in {phrase}. My speciality is cybersecurity, so ask me about passwords, phishing or privacy whenever you like.");
            }

            _user.FavouriteTopic = topic.Key;
            _currentTopic = topic;
            _user.RecordTopic(topic.Key);
            NotifyMemoryChanged();

            return new ChatReply($"Great! I'll remember that you're interested in {topic.DisplayName}. {topic.Intro} Say 'give me a tip' and I'll share one.");
        }

        /// <summary>Answers "what do you remember about me?" using stored memory.</summary>
        private ChatReply HandleRecall(string raw, string text)
        {
            if (!RecallPattern.IsMatch(text))
            {
                return null;
            }

            var lines = new List<string> { $"Your name is {_user.Name}." };

            if (!string.IsNullOrEmpty(_user.FavouriteTopic))
            {
                lines.Add($"You told me you're interested in {TopicName(_user.FavouriteTopic)}.");
            }

            if (_user.TopicsDiscussed.Count > 0)
            {
                string discussed = string.Join(", ", _user.TopicsDiscussed.Select(TopicName));
                lines.Add($"We've talked about: {discussed}.");
            }

            if (!string.IsNullOrEmpty(_user.LastSentiment))
            {
                lines.Add($"The last time you shared a feeling, you seemed {_user.LastSentiment.ToLowerInvariant()}.");
            }

            if (lines.Count == 1)
            {
                lines.Add("Tell me what you're interested in, like 'I'm interested in privacy', and I'll remember it.");
            }

            return new ChatReply(string.Join(" ", lines));
        }

        /// <summary>Greetings, "how are you", purpose, what can I ask, thanks and goodbye.</summary>
        private ChatReply HandleSmallTalk(string raw, string text)
        {
            foreach (var (pattern, replies) in SmallTalk)
            {
                if (pattern.IsMatch(text))
                {
                    string reply = replies[Random.Shared.Next(replies.Length)];
                    return new ChatReply(reply.Replace("{name}", _user.Name));
                }
            }

            return null;
        }

        /// <summary>
        /// Handles cybersecurity topics, sentiment, and follow-ups such as
        /// "give me another tip" or "tell me more" (which continue the current topic).
        /// </summary>
        private ChatReply HandleTopicConversation(string raw, string text)
        {
            Sentiment? mood = _sentiment.Detect(text);
            bool isFollowUp = FollowUpPattern.IsMatch(text);
            bool wantsTip = TipPattern.IsMatch(text);
            bool wantsDetail = DetailPattern.IsMatch(text) && !wantsTip;

            Topic topic = FindTopic(text);

            // Follow-ups or confusion without a named topic continue the current one.
            if (topic == null && (isFollowUp || mood == Sentiment.Confused))
            {
                if (_currentTopic == null)
                {
                    return new ChatReply($"Happy to go deeper, {_user.Name}! Which topic would you like to explore? For example passwords, phishing, privacy or safe browsing.");
                }

                topic = _currentTopic;
            }

            // A feeling with no topic still gets a tip right away (no need to ask again).
            if (topic == null && mood.HasValue)
            {
                if (mood == Sentiment.Positive)
                {
                    RememberMood(mood);
                    return new ChatReply($"{_sentiment.GetOpener(mood.Value, _user.Name)} Ask me about any topic, or say 'give me a tip'.", mood);
                }

                topic = _currentTopic ?? KnowledgeBase.General;
            }

            if (topic == null)
            {
                return null;
            }

            bool useDetail = wantsDetail || mood == Sentiment.Confused;
            string content = useDetail
                ? NextFromDeck($"{topic.Key}:detail", topic.Details)
                : NextFromDeck($"{topic.Key}:tip", topic.Tips);

            var parts = new List<string>();

            if (mood.HasValue)
            {
                parts.Add(_sentiment.GetOpener(mood.Value, _user.Name));
            }
            else if (_currentTopic != topic && topic != KnowledgeBase.General)
            {
                parts.Add($"Let's talk about {topic.DisplayName}.");
            }

            parts.Add(content);

            string personal = Personalise(topic);
            if (personal != null)
            {
                parts.Add(personal);
            }

            // Update memory and conversation state.
            _currentTopic = topic;
            if (topic != KnowledgeBase.General)
            {
                _user.RecordTopic(topic.Key);
            }
            RememberMood(mood);
            NotifyMemoryChanged();

            return new ChatReply(string.Join("\n\n", parts), mood);
        }

        // ---------- Helpers ----------

        private ChatReply Fallback()
        {
            string topics = string.Join(", ", KnowledgeBase.Topics.Values.Select(t => t.DisplayName));
            return new ChatReply($"I'm not sure I understand, {_user.Name}. Can you try rephrasing? You can ask me about {topics}.");
        }

        /// <summary>Returns the topic with the most keyword hits (first one wins ties), or null.</summary>
        private static Topic FindTopic(string text)
        {
            Topic best = null;
            int bestScore = 0;

            foreach (Topic topic in KnowledgeBase.Topics.Values)
            {
                int score = topic.MatchScore(text);
                if (score > bestScore)
                {
                    best = topic;
                    bestScore = score;
                }
            }

            return best;
        }

        /// <summary>
        /// Random selection without immediate repeats: items are dealt from a shuffled
        /// deck, and the deck is reshuffled once it runs out.
        /// </summary>
        private string NextFromDeck(string key, IReadOnlyList<string> items)
        {
            if (!_decks.TryGetValue(key, out Queue<int> deck) || deck.Count == 0)
            {
                deck = new Queue<int>(Enumerable.Range(0, items.Count).OrderBy(_ => Random.Shared.Next()));
                _decks[key] = deck;
            }

            return items[deck.Dequeue()];
        }

        /// <summary>Uses memory to personalise a reply based on the user's favourite topic.</summary>
        private string Personalise(Topic topic)
        {
            _replyCount++;

            if (string.IsNullOrEmpty(_user.FavouriteTopic))
            {
                return null;
            }

            if (_user.FavouriteTopic == topic.Key)
            {
                return $"As someone interested in {topic.DisplayName}, {topic.InterestNote}";
            }

            // Every third reply, gently recall the favourite topic.
            if (_replyCount % 3 == 0 && KnowledgeBase.Topics.TryGetValue(_user.FavouriteTopic, out Topic favourite))
            {
                return $"By the way, {_user.Name}, since you're interested in {favourite.DisplayName}, {favourite.InterestNote}";
            }

            return null;
        }

        private void RememberMood(Sentiment? mood)
        {
            if (mood.HasValue)
            {
                _user.LastSentiment = mood.Value.ToString();
            }
        }

        private static string TopicName(string keyOrText) =>
            KnowledgeBase.Topics.TryGetValue(keyOrText, out Topic topic) ? topic.DisplayName : keyOrText;

        private static string Normalise(string input) =>
            Regex.Replace(input.ToLowerInvariant().Replace('’', '\''), @"\s+", " ").Trim();

        private void NotifyMemoryChanged() => MemoryChanged?.Invoke(_user);
    }
}
