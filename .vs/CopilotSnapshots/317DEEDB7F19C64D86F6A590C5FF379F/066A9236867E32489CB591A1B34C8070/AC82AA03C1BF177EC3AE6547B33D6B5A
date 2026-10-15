using System.Media;

namespace CybersecurityAwarenessBot
{
    public static class VoiceGreeting
    {
        public static void Play()
        {
            try
            {
                string audioPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    "greetings.wav");

                if (File.Exists(audioPath))
                {
                    using SoundPlayer player = new SoundPlayer(audioPath);
                    player.PlaySync();
                }
                else
                {
                    ConsoleUI.DisplayBotMessage("Voice greeting not found (expected at Assets\\greetings.wav). Continuing without audio.");
                }
            }
            catch (Exception ex)
            {
                // Do not crash the app for audio playback issues
                ConsoleUI.DisplayBotMessage($"Unable to play voice greeting: {ex.Message}");
            }
        }
    }
}