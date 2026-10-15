using System;
using System.IO;
using System.Media;
using System.Text;
using System.Speech.Synthesis;

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

                // Ensure Assets folder exists in the output
                string audioDir = Path.GetDirectoryName(audioPath) ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
                if (!Directory.Exists(audioDir))
                {
                    Directory.CreateDirectory(audioDir);
                }

                // If the WAV is missing, generate a short default beep WAV so audio works out-of-the-box
                if (!File.Exists(audioPath))
                {
                    try
                    {
                        CreateDefaultWav(audioPath);
                        ConsoleUI.DisplayBotMessage($"Generated default greeting at: {audioPath}");
                    }
                    catch (Exception genEx)
                    {
                        ConsoleUI.DisplayBotMessage($"Voice greeting not found (expected at Assets\\greetings.wav). Attempting speech synthesis fallback...\nFailed to create default WAV: {genEx.Message}");

                        try
                        {
                            using var synth = new SpeechSynthesizer();
                            synth.Speak("Welcome to the Cybersecurity Awareness Bot.");
                        }
                        catch
                        {
                            ConsoleUI.DisplayBotMessage("No TTS engine available. Continuing without audio.");
                        }

                        return;
                    }
                }

                using SoundPlayer player = new SoundPlayer(audioPath);
                player.PlaySync();
            }
            catch (Exception ex)
            {
                // Do not crash the app for audio playback issues
                ConsoleUI.DisplayBotMessage($"Unable to play voice greeting: {ex.Message}");

                // Try speech synthesis fallback
                try
                {
                    using var synth = new SpeechSynthesizer();
                    synth.Speak("Welcome to the Cybersecurity Awareness Bot.");
                }
                catch { /* swallow */ }
            }
        }

        // Create a tiny PCM WAV with a short tone
        private static void CreateDefaultWav(string path)
        {
            const int sampleRate = 44100;
            const short bitsPerSample = 16;
            const short channels = 1;
            double durationSeconds = 0.9;
            int samples = (int)(sampleRate * durationSeconds);

            double freq = 880.0; // frequency of the tone (A5)
            double amplitude = 0.25 * short.MaxValue;

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs, Encoding.ASCII))
            {
                int byteRate = sampleRate * channels * bitsPerSample / 8;
                short blockAlign = (short)(channels * bitsPerSample / 8);
                int subChunk2Size = samples * channels * bitsPerSample / 8;
                int chunkSize = 36 + subChunk2Size;

                bw.Write(Encoding.ASCII.GetBytes("RIFF"));
                bw.Write(chunkSize);
                bw.Write(Encoding.ASCII.GetBytes("WAVE"));

                // fmt subchunk
                bw.Write(Encoding.ASCII.GetBytes("fmt "));
                bw.Write(16); // Subchunk1Size for PCM
                bw.Write((short)1); // AudioFormat = 1 (PCM)
                bw.Write(channels);
                bw.Write(sampleRate);
                bw.Write(byteRate);
                bw.Write(blockAlign);
                bw.Write(bitsPerSample);

                // data subchunk
                bw.Write(Encoding.ASCII.GetBytes("data"));
                bw.Write(subChunk2Size);

                // Write samples
                for (int i = 0; i < samples; i++)
                {
                    double t = (double)i / sampleRate;
                    short sample = (short)(amplitude * Math.Sin(2.0 * Math.PI * freq * t));
                    bw.Write(sample);
                }
            }
        }
    }
}
