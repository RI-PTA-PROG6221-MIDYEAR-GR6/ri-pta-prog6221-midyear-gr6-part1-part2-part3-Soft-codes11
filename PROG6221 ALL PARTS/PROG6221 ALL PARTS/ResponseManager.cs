namespace CybersecurityAwarenessBot
{
    public static class ResponseManager
    {
        public static string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            if (input.Contains("how are you"))
            {
                return "I'm doing great! I'm ready to help you learn more about staying safe online.";
            }

            if (input.Contains("purpose") || input.Contains("what do you do"))
            {
                return "My purpose is to provide cybersecurity awareness information and help you understand safer online practices.";
            }

            if (input.Contains("what can i ask") || input.Contains("what can i ask you about"))
            {
                return "You can ask me about password safety, phishing, safe browsing, malware, suspicious links, account security and social engineering.";
            }

            if (input.Contains("password"))
            {
                return "Use a strong, unique password for each account. Avoid easily guessed information and consider using a reputable password manager.";
            }

            if (input.Contains("phishing"))
            {
                return "Phishing is an attempt to trick you into revealing sensitive information. Be cautious of unexpected messages, suspicious links, urgent requests and unfamiliar senders.";
            }

            if (input.Contains("safe browsing") || input.Contains("browsing"))
            {
                return "For safer browsing, check website addresses carefully, avoid suspicious downloads and links, keep your browser updated and use secure websites.";
            }

            if (input.Contains("malware"))
            {
                return "Malware is malicious software designed to harm systems or steal information. Keep software updated, use trusted security tools and avoid suspicious downloads.";
            }

            if (input.Contains("suspicious link") || input.Contains("suspicious links"))
            {
                return "Do not click suspicious links immediately. Check the sender, inspect the destination carefully and visit the official website directly when possible.";
            }

            if (input.Contains("social engineering"))
            {
                return "Social engineering manipulates people into giving away information or performing unsafe actions. Be cautious of pressure, urgency and requests for confidential information.";
            }

            if (input.Contains("account security") || input.Contains("secure my account"))
            {
                return "Protect your accounts with strong unique passwords, multi-factor authentication, updated recovery information and careful handling of login requests.";
            }

            return "I didn't quite understand that. Could you rephrase your question? You can ask about passwords, phishing, safe browsing, malware, suspicious links, account security or social engineering.";
        }
    }
}