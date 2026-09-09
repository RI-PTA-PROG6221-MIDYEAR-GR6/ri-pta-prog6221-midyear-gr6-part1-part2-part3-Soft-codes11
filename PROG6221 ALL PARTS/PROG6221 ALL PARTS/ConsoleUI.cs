using System;
using System.Threading;

namespace CybersecurityAwarenessBot
{
    public static class ConsoleUI
    {
        // Made public so `TypingDelayMs` is accessible from other classes/files if DisplayBotMessage
        // or similar methods are moved outside this class.
        public const int TypingDelayMs = 8;

        public static void DisplayLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine();
            Console.WriteLine("====================================================");
            Console.WriteLine("             CYBERSECURITY AWARENESS                ");
            Console.WriteLine("====================================================");
            Console.WriteLine();
            Console.WriteLine("              .-----------------.                   ");
            Console.WriteLine("             /   CYBER SHIELD    \\                  ");
            Console.WriteLine("            |       _____        |                  ");
            Console.WriteLine("            |      /     \\       |                  ");
            Console.WriteLine("            |     |  ^ ^  |      |                  ");
            Console.WriteLine("            |     |   _   |      |                  ");
            Console.WriteLine("            |      \\_____/       |                  ");
            Console.WriteLine("             \\                 /                   ");
            Console.WriteLine("              '---------------'                    ");
            Console.WriteLine();
            Console.WriteLine("====================================================");

            Console.ResetColor();
        }

        public static void DisplayHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"  {title}");
            Console.WriteLine("----------------------------------------------------");

            Console.ResetColor();
        }

        public static void DisplayBotMessage(string message, bool typing = true)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (typing)
            {
                Console.Write("CyberBot: ");
                foreach (char c in message)
                {
                    Console.Write(c);
                    Thread.Sleep(ConsoleUI.TypingDelayMs); // Use the class name to access the constant
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"CyberBot: {message}");
            }
            Console.ResetColor();
        }

        public static void DisplayUserMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"You: {message}");
            Console.ResetColor();
        }

        public static void DisplayPrompt()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");
            Console.ResetColor();
        }
    }
}