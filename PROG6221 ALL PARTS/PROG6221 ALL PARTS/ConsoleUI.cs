namespace CybersecurityAwarenessBot
{
    public static class ConsoleUI
    {
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

            string logo = @"        /\/\               /\        /\         /\          /\         /\         /\                 /\                /\       
 /‾‾‾‾‾‾  \ ‾‾‾‾‾‾| |‾‾‾‾‾‾  \‾‾‾‾‾‾‾  \ |‾‾‾‾|‾  \ \‾‾‾‾‾‾‾  \\‾‾‾‾‾‾‾  \ /‾‾‾‾‾‾  \‾‾‾‾‾‾\  |‾‾‾‾|‾  \‾‾‾| |‾‾‾‾|    \ ‾‾‾‾‾‾|
/     /‾‾‾/‾‾‾‾   | |    |   /|        / |    |   /  \        / |        //     /‾‾‾|    |  \ |    |   /   | |    |    /‾‾‾‾   |
\    /  /\\   |   | |    |  ‾\|    ‾‾)‾  |       ‾\  /‾‾‾‾‾  /  |    ‾‾)‾ \    /  /\|    |   \|       ‾\   | |    |    \   |   |
/‾   ‾‾‾ /‾‾  |   |/‾‾‾‾‾‾   /‾   ‾‾‾‾‾\/‾‾‾‾‾‾   /  \   ‾‾‾‾‾\/‾   ‾‾‾‾‾\/‾   ‾‾‾ /|    |   /‾‾‾‾‾‾   /   /‾‾    ‾‾‾\/‾‾  |   |
‾‾‾‾‾‾‾‾‾‾‾‾‾‾˙‾‾‾˙‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾    ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾  ‾‾‾‾ ‾‾‾‾‾‾‾‾‾‾‾‾‾˙‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾˙‾‾‾˙";

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

        public static void DisplayBotMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"CyberBot: {message}");
            Console.ResetColor();
        }

        public static void DisplayUserMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"You: {message}");
            Console.ResetColor();
        }
    }
}