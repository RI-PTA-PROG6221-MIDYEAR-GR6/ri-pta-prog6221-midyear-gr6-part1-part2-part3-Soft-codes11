using CybersecurityAwarenessBot;

           /* string logo = @"        /\/\               /\        /\         /\          /\         /\         /\                 /\                /\       
 /‾‾‾‾‾‾  \ ‾‾‾‾‾‾| |‾‾‾‾‾‾  \‾‾‾‾‾‾‾  \ |‾‾‾‾|‾  \ \‾‾‾‾‾‾‾  \\‾‾‾‾‾‾‾  \ /‾‾‾‾‾‾  \‾‾‾‾‾‾\  |‾‾‾‾|‾  \‾‾‾| |‾‾‾‾|    \ ‾‾‾‾‾‾|
/     /‾‾‾/‾‾‾‾   | |    |   /|        / |    |   /  \        / |        //     /‾‾‾|    |  \ |    |   /   | |    |    /‾‾‾‾   |
\    /  /\\   |   | |    |  ‾\|    ‾‾)‾  |       ‾\  /‾‾‾‾‾  /  |    ‾‾)‾ \    /  /\|    |   \|       ‾\   | |    |    \   |   |
/‾   ‾‾‾ /‾‾  |   |/‾‾‾‾‾‾   /‾   ‾‾‾‾‾\/‾‾‾‾‾‾   /  \   ‾‾‾‾‾\/‾   ‾‾‾‾‾\/‾   ‾‾‾ /|    |   /‾‾‾‾‾‾   /   /‾‾    ‾‾‾\/‾‾  |   |
‾‾‾‾‾‾‾‾‾‾‾‾‾‾˙‾‾‾˙‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾    ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾  ‾‾‾‾ ‾‾‾‾‾‾‾‾‾‾‾‾‾˙‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾˙‾‾‾˙";

            try {
                SoundPlayer sound = new SoundPlayer("AI.wav");
                sound.Play();
            } catch (Exception ex) {
                Console.WriteLine("Error!! Audio not found " + ex.Message);
            }
            SoundPlayer player = new SoundPlayer("AI.wav");
            player.Play();
            Console.WriteLine(logo);
            Console.WriteLine("Welcome to the Example Program!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            *
            *
            */

       namespace CybersecurityAwarenessBot
    {
        internal class Program
        {
            static void Main(string[] args)
            {
                Console.Title = "Cybersecurity Awareness Bot";

                ConsoleUI.DisplayLogo();

                ConsoleUI.DisplayBotMessage(
                    "Welcome to the Cybersecurity Awareness Bot!");

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
