using System.Media;

namespace CybersecurityAwarenessBot
{
    public static class VoiceGreeting
    {
        public static void Play()
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
                Console.WriteLine("Voice greeting could not be found.");
            }
        }
    }
}