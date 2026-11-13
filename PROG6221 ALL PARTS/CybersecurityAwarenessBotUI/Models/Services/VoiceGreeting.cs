using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace CybersecurityAwarenessBot.Services
{
    /// <summary>
    /// Plays the welcome message from Assets\greetings.wav.
    /// </summary>
    public static class VoiceGreeting
    {
        public static Task<string> PlayAsync()
        {
            return Task.Run(() =>
            {
                string audioPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    "greetings.wav");

                try
                {
                    // Return an error only when the file is missing.
                    if (!File.Exists(audioPath))
                    {
                        return $"Voice greeting not found. Expected file: {audioPath}";
                    }

                    using SoundPlayer player = new SoundPlayer(audioPath);

                    // Load the WAV file before playback.
                    player.Load();
                    player.PlaySync();

                    return "Voice greeting played successfully.";
                }
                catch (Exception ex)
                {
                    return $"Unable to play voice greeting: {ex.Message}";
                }
            });
        }
    }
}