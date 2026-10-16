using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CybersecurityAwarenessBot.Data;
using CybersecurityAwarenessBot.Helpers;
using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// The chat window. This class only handles the user interface (showing bubbles,
    /// the typing effect, the sidebar). All of the "thinking" lives in ChatEngine.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string BotName = "CyberBot";

        // Accepts letters (any language), spaces, hyphens and apostrophes, 1-30 characters.
        private static readonly Regex ValidName = new(@"^[\p{L}][\p{L} '\-]{0,29}$", RegexOptions.Compiled);

        private readonly ObservableCollection<ChatMessage> _messages = new();
        private User _user;
        private ChatEngine _engine;
        private bool _isBusy;

        public MainWindow()
        {
            InitializeComponent();

            ChatList.ItemsSource = _messages;
            WelcomeLogo.Text = AsciiArt.Logo;
            SidebarShield.Text = AsciiArt.Shield;
            WelcomeVoiceStatus.Text = "Playing voice greeting...";

            Loaded += async (_, _) =>
            {
                NameBox.Focus();
                await PlayVoiceGreetingAsync();
            };
        }

        // ---------- Start-up ----------

        private async Task PlayVoiceGreetingAsync()
        {
            string error = await VoiceGreeting.PlayAsync();
            string status = error ?? "Voice greeting played";

            VoiceStatus.Text = status;
            WelcomeVoiceStatus.Text = status;
        }

        private void NameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartButton_Click(sender, e);
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();

            if (!ValidName.IsMatch(name))
            {
                NameError.Text = string.IsNullOrEmpty(name)
                    ? "Name cannot be empty. Please enter your name."
                    : "Please use letters only (spaces, hyphens and apostrophes are fine).";
                NameError.Visibility = Visibility.Visible;
                NameBox.Focus();
                return;
            }

            _user = new User(name);
            _engine = new ChatEngine(_user);
            _engine.MemoryChanged += RefreshMemoryPanel;   // delegate-based event: sidebar updates itself

            WelcomeOverlay.Visibility = Visibility.Collapsed;
            RefreshMemoryPanel(_user);
            SetBusy(true);   // block input and topic buttons while the bot introduces itself

            await BotSayAsync($"Nice to meet you, {_user.Name}. I'm your Cybersecurity Awareness Bot.");
            await BotSayAsync("I can help you learn about safer online behaviour. Try asking me about passwords, phishing, privacy or safe browsing, or tell me how you're feeling.");

            SetBusy(false);
        }

        // ---------- Sending messages ----------

        private async void SendButton_Click(object sender, RoutedEventArgs e) =>
            await SendAsync(InputBox.Text);

        private async void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !_isBusy)
            {
                e.Handled = true;
                await SendAsync(InputBox.Text);
            }
        }

        private async void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Tag: string question })
            {
                await SendAsync(question);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isBusy)
            {
                return;
            }

            _messages.Clear();
            _messages.Add(new ChatMessage(BotName, "Chat cleared. I still remember you, so ask me anything!", true));
        }

        private async Task SendAsync(string input)
        {
            if (_isBusy || _engine == null)
            {
                return;
            }

            input = input?.Trim() ?? string.Empty;
            SetBusy(true);

            // Empty input: show a gentle nudge instead of a blank chat bubble.
            if (input.Length == 0)
            {
                await BotSayAsync(_engine.GetReply(input).Text);
                SetBusy(false);
                return;
            }

            _messages.Add(new ChatMessage(_user.Name, input, false));
            InputBox.Clear();
            ScrollToBottom();

            TypingText.Visibility = Visibility.Visible;
            await Task.Delay(550);   // short pause so it feels conversational

            ChatReply reply;
            try
            {
                reply = _engine.GetReply(input);
            }
            catch (Exception)
            {
                // The bot must never crash on unexpected input.
                reply = new ChatReply("Sorry, something went wrong on my side. Could you try rephrasing that?");
            }

            TypingText.Visibility = Visibility.Hidden;
            await BotSayAsync(reply.Text);

            ShowMood(reply.Mood);
            SetBusy(false);
        }

        // ---------- Bot output ----------

        /// <summary>Adds a bot bubble and reveals its text gradually (typing effect).</summary>
        private async Task BotSayAsync(string text)
        {
            var message = new ChatMessage(BotName, string.Empty, true);
            _messages.Add(message);

            // Reveal in small chunks; long replies use bigger chunks so they never feel slow.
            int step = Math.Max(1, text.Length / 90);
            for (int i = 0; i < text.Length; i += step)
            {
                message.Text = text.Substring(0, Math.Min(i + step, text.Length));
                ChatScroll.ScrollToEnd();
                await Task.Delay(15);
            }

            message.Text = text;
            ScrollToBottom();
        }

        private void ScrollToBottom() =>
            Dispatcher.InvokeAsync(() => ChatScroll.ScrollToEnd(), System.Windows.Threading.DispatcherPriority.Background);

        private void SetBusy(bool busy)
        {
            _isBusy = busy;
            SendButton.IsEnabled = !busy;
            InputBox.IsEnabled = !busy;

            if (!busy)
            {
                InputBox.Focus();
            }
        }

        // ---------- Sidebar ----------

        private void RefreshMemoryPanel(User user)
        {
            string interest = string.IsNullOrEmpty(user.FavouriteTopic)
                ? "not shared yet"
                : (KnowledgeBase.Topics.TryGetValue(user.FavouriteTopic, out Topic topic) ? topic.DisplayName : user.FavouriteTopic);

            string explored = user.TopicsDiscussed.Count == 0
                ? "none yet"
                : string.Join(", ", user.TopicsDiscussed.Select(k => KnowledgeBase.Topics[k].DisplayName));

            MemoryText.Text =
                $"Name: {user.Name}\n" +
                $"Interest: {interest}\n" +
                $"Topics explored: {explored}";
        }

        private void ShowMood(Sentiment? mood)
        {
            if (!mood.HasValue)
            {
                return;
            }

            string emoji = mood.Value switch
            {
                Sentiment.Worried => "😟",
                Sentiment.Frustrated => "😠",
                Sentiment.Overwhelmed => "😓",
                Sentiment.Confused => "😕",
                Sentiment.Curious => "🤔",
                _ => "😊"
            };

            MoodText.Text = $"Mood detected: {mood.Value} {emoji}";
        }
    }
}
