using System;
using System.Threading;

namespace CybersecurityAwarenessBot
{
    public static class ConsoleUI
    {
        private const int TypingDelayMs = 8;

        public static void DisplayBotMessage(string message, bool typing = true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (typing)
            {
                Console.Write("CyberBot: ");
                foreach (char c in message)
                {
                    Console.Write(c);
                    Thread.Sleep(TypingDelayMs);
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"CyberBot: {message}");
            }
            Console.ResetColor();
        }
    }
}
