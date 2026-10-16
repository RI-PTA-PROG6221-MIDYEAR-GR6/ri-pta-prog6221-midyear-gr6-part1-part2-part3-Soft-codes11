namespace CybersecurityAwarenessBot.Helpers
{
    /// <summary>
    /// The Part 1 ASCII art, carried into the GUI. Shown in a monospaced
    /// TextBlock so the characters line up exactly as they did in the console.
    /// </summary>
    public static class AsciiArt
    {
        public const string Logo = """
            ====================================================
                         CYBERSECURITY AWARENESS
            ====================================================

                          .-----------------.
                         /   CYBER SHIELD    \
                        |       _____        |
                        |      /     \       |
                        |     |  ^ ^  |      |
                        |     |   _   |      |
                        |      \_____/       |
                         \                 /
                          '---------------'

            ====================================================
            """;

        /// <summary>Compact version for the sidebar.</summary>
        public const string Shield = """
              .-------------.
             /  CYBER SHIELD \
            |     ( ^ ^ )     |
            |      \___/      |
             \               /
              '-------------'
            """;
    }
}
