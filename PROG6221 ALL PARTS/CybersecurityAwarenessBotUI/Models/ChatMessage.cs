using System.ComponentModel;

namespace CybersecurityAwarenessBot.Models
{
    /// <summary>
    /// A single chat bubble. Implements INotifyPropertyChanged so the bubble
    /// updates live while the typing effect reveals the text.
    /// </summary>
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _text;

        public string Sender { get; }
        public bool IsBot { get; }
        public string Time { get; } = DateTime.Now.ToString("HH:mm");

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
            }
        }

        public ChatMessage(string sender, string text, bool isBot)
        {
            Sender = sender;
            _text = text;
            IsBot = isBot;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
