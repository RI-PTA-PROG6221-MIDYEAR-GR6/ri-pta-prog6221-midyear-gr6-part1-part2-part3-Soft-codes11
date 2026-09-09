using CybersecurityAwarenessBot;
using System;

namespace CybersecurityAwarenessBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Cybersecurity Awareness Bot";

            // Play voice greeting (graceful fallback if missing)
            VoiceGreeting.Play();

            ConsoleUI.DisplayLogo();

            ConsoleUI.DisplayBotMessage("Welcome to the Cybersecurity Awareness Bot!", typing: false);

            Console.WriteLine();
            Console.Write("Please enter your name: ");

            string name = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Name cannot be empty. Please enter your name.");
                Console.ResetColor();

                Console.Write("Please enter your name: ");
                name = Console.ReadLine();
            }

            User user = new User(name.Trim());

            Chatbot chatbot = new Chatbot(user);

            chatbot.StartConversation();
        }
    }
}
