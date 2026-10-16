using System.Windows;
using System.Windows.Threading;

namespace CybersecurityAwarenessBot
{
    /// <summary>
    /// Application entry point (replaces the console Program.cs from Part 1).
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Last line of defence: an unexpected error shows a friendly message
        /// instead of crashing the whole application.
        /// </summary>
        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Something unexpected happened, but the bot is still running.\n\nDetails: {e.Exception.Message}",
                "Cybersecurity Awareness Bot",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            e.Handled = true;
        }
    }
}
