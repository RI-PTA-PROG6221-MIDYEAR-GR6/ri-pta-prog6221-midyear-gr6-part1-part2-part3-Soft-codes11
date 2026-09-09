using System;

namespace CybersecurityAwarenessBot
{
    public class Chatbot
    {
        private readonly User user;

        public Chatbot(User user)
        {
            this.user = user;
        }

        public void StartConversation()
        {
            ConsoleUI.DisplayHeader($"Welcome, {user.Name}!");

            ConsoleUI.DisplayBotMessage($"Nice to meet you, {user.Name}. I'm your Cybersecurity Awareness Bot.");
            ConsoleUI.DisplayBotMessage("I can help you learn about safer online behaviour.");
            ConsoleUI.DisplayBotMessage("Try asking me about passwords, phishing, or safe browsing.");

            while (true)
            {
                Console.WriteLine();
                ConsoleUI.DisplayBotMessage("Type your question or type 'exit' to leave.", typing: false);
                ConsoleUI.DisplayPrompt();

                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    ConsoleUI.DisplayBotMessage("Please enter a question so I can help you.");
                    continue;
                }

                input = input.Trim();

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    ConsoleUI.DisplayBotMessage($"Thanks for chatting with me, {user.Name}. Stay safe online!");
                    break;
                }

                ConsoleUI.DisplayUserMessage(input);

                string response = ResponseManager.GetResponse(input);

                ConsoleUI.DisplayBotMessage(response, typing: true);
            }
        }
    }
}
