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

        private static readonly Regex ReminderDatePattern = new(
            @"\b(?:in\s+(?<days>\d+)\s+days?|after\s+(?<daysAfter>\d+)\s+days?)\b|\b(?<when>today|tomorrow)\b",
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
        private readonly TaskService _taskService = new();
        private readonly ActivityLogService _activityLog = new();
        private readonly QuizService _quizService = new();

        /// <summary>Shuffled "decks" so random tips don't repeat until the whole list has been used.</summary>
        private readonly Dictionary<string, Queue<int>> _decks = new();

        private Topic _currentTopic;
        private int _replyCount;
        private int _quizIndex = -1;
        private int _quizScore;
        private bool _quizActive;

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
                HandlePart3Commands,
                HandleTopicConversation
            };
        }

        public IReadOnlyList<CyberTask> Tasks => _taskService.Tasks;
        public IReadOnlyList<ActivityEntry> ActivityEntries => _activityLog.Entries;
        public QuizQuestion CurrentQuizQuestion => _quizIndex >= 0 && _quizIndex < _quizService.Questions.Count
            ? _quizService.Questions[_quizIndex]
            : null;
        public bool QuizActive => _quizActive;
        public int QuizScore => _quizScore;
        public int QuizQuestionNumber => _quizIndex + 1;
        public int QuizTotalQuestions => _quizService.Questions.Count;

        public event Action StateChanged;

        public CyberTask AddTask(string title, string description, DateTime? reminderDate)
        {
            CyberTask task = _taskService.Add(title, description, reminderDate);
            _activityLog.Record(reminderDate.HasValue
                ? $"Task added from GUI: '{task.Title}' with reminder on {reminderDate.Value:dd MMM yyyy}"
                : $"Task added from GUI: '{task.Title}' without a reminder");
            StateChanged?.Invoke();
            return task;
        }

        public bool UpdateTask(Guid id, string action)
        {
            if (action.Equals("complete", StringComparison.OrdinalIgnoreCase))
            {
                bool completed = _taskService.Complete(id);
                if (completed)
                {
                    _activityLog.Record("Task marked as completed");
                    StateChanged?.Invoke();
                }

                return completed;
            }

            if (action.Equals("delete", StringComparison.OrdinalIgnoreCase))
            {
                bool deleted = _taskService.Delete(id);
                if (deleted)
                {
                    _activityLog.Record("Task deleted");
                    StateChanged?.Invoke();
                }

                return deleted;
            }

            return false;
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

        /// <summary>Recognises Part 3 task, reminder, quiz and activity-log commands.</summary>
        private ChatReply HandlePart3Commands(string raw, string text)
        {
            if (text.Contains("show activity") || text.Contains("what have you done") || text.Contains("activity log"))
            {
                _activityLog.Record("Activity log viewed");
                IReadOnlyList<ActivityEntry> recent = _activityLog.Recent(8);
                string summary = recent.Count == 0
                    ? "I have not recorded any actions yet. Try adding a task or starting the quiz."
                    : "Here is a summary of recent actions:\n" + string.Join("\n", recent.Select((entry, index) => $"{index + 1}. {entry.Description} ({entry.Timestamp:dd MMM HH:mm})"));
                return new ChatReply(summary);
            }

            if (text.Contains("start quiz") || text.Contains("begin quiz") || text == "quiz" || text.Contains("cybersecurity quiz"))
            {
                StartQuiz();
                return new ChatReply(GetQuizQuestionText());
            }

            if (_quizActive && TryAnswerQuiz(text, out string quizReply))
            {
                return new ChatReply(quizReply);
            }

            if (text.Contains("add task") || text.Contains("create task") || text.Contains("remind me to") || text.Contains("set a reminder"))
            {
                string title = ExtractTaskTitle(raw);
                DateTime? reminder = ParseReminderDate(text);
                CyberTask task = _taskService.Add(title, $"Cybersecurity action: {title}.", reminder);
                _activityLog.Record(reminder.HasValue
                    ? $"Task added: '{task.Title}' with reminder on {reminder.Value:dd MMM yyyy}"
                    : $"Task added: '{task.Title}' without a reminder");
                StateChanged?.Invoke();

                string reminderText = reminder.HasValue
                    ? $"Reminder set for {reminder.Value:dd MMM yyyy}."
                    : "Would you like to set a reminder? Try 'remind me in 3 days'.";
                return new ChatReply($"Task added: '{task.Title}'. {reminderText}");
            }

            if (text.Contains("show tasks") || text.Contains("my tasks") || text.Contains("list tasks"))
            {
                if (_taskService.Tasks.Count == 0)
                {
                    return new ChatReply("You do not have any tasks yet. Try 'add task to enable two-factor authentication'.");
                }

                string taskList = string.Join("\n", _taskService.Tasks.Select((task, index) =>
                    $"{index + 1}. [{task.StatusText}] {task.Title} — {task.ReminderText}"));
                return new ChatReply("Your cybersecurity tasks:\n" + taskList);
            }

            return null;
        }

        private void StartQuiz()
        {
            _quizActive = true;
            _quizIndex = 0;
            _quizScore = 0;
            _activityLog.Record($"Quiz started ({_quizService.Questions.Count} questions)");
            StateChanged?.Invoke();
        }

        private string GetQuizQuestionText()
        {
            QuizQuestion question = CurrentQuizQuestion;
            string options = string.Join("\n", question.Options.Select((option, index) => $"{(char)('A' + index)}) {option}"));
            return $"Question {_quizIndex + 1} of {_quizService.Questions.Count}: {question.Question}\n{options}";
        }

        private bool TryAnswerQuiz(string text, out string response)
        {
            response = string.Empty;
            QuizQuestion question = CurrentQuizQuestion;
            int answerIndex = text switch
            {
                "a" or "a)" or "option a" => 0,
                "b" or "b)" or "option b" => 1,
                "c" or "c)" or "option c" => 2,
                "d" or "d)" or "option d" => 3,
                _ when text.Contains("true") => 0,
                _ when text.Contains("false") => 1,
                _ => -1
            };

            if (answerIndex < 0 || answerIndex >= question.Options.Count)
            {
                response = "Please answer the quiz with A, B, C or D (or True/False).";
                return true;
            }

            bool correct = answerIndex == question.CorrectIndex;
            if (correct)
            {
                _quizScore++;
            }

            response = (correct ? "Correct! " : "Not quite. ") + question.Explanation;
            _quizIndex++;

            if (_quizIndex >= _quizService.Questions.Count)
            {
                _quizActive = false;
                _activityLog.Record($"Quiz completed: {_quizScore}/{_quizService.Questions.Count}");
                response += $"\n\nQuiz complete: {_quizScore}/{_quizService.Questions.Count}. " +
                    (_quizScore >= 10 ? "Excellent work — you are a cybersecurity pro!" : "Keep learning to stay safer online!");
            }
            else
            {
                response += "\n\n" + GetQuizQuestionText();
            }

            StateChanged?.Invoke();
            return true;
        }

        private static string ExtractTaskTitle(string raw)
        {
            string title = Regex.Replace(raw, @"^(please\s+)?(add|create)\s+(a\s+)?task\s*(to|for)?\s*", string.Empty, RegexOptions.IgnoreCase).Trim(' ', '.', '?');
            title = Regex.Replace(title, @"^(can you|please)\s+remind me to\s+", string.Empty, RegexOptions.IgnoreCase).Trim(' ', '.', '?');
            title = Regex.Replace(title, @"^remind me to\s+", string.Empty, RegexOptions.IgnoreCase).Trim(' ', '.', '?');
            title = Regex.Replace(title, @"\b(in\s+\d+\s+days?|tomorrow|today)\b", string.Empty, RegexOptions.IgnoreCase).Trim(' ', '.', '?');
            return string.IsNullOrWhiteSpace(title) ? "Review cybersecurity settings" : title;
        }

        private static DateTime? ParseReminderDate(string text)
        {
            Match match = ReminderDatePattern.Match(text);
            if (!match.Success)
            {
                return null;
            }

            if (match.Groups["when"].Value.Equals("tomorrow", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.Today.AddDays(1);
            }

            if (match.Groups["when"].Value.Equals("today", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.Today;
            }

            string daysText = match.Groups["days"].Success ? match.Groups["days"].Value : match.Groups["daysAfter"].Value;
            return int.TryParse(daysText, out int days) ? DateTime.Today.AddDays(days) : null;
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
