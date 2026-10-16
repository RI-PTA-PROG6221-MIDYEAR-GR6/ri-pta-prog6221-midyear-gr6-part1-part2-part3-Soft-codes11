using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityAwarenessBot
{
    public static class ResponseManager
    {
        private static readonly Random _rng = new();

        private static readonly Dictionary<string[], string[]> TopicResponses = new()
        {
            { new[] { "password", "pass", "passwd" }, new[] {
                "Use a strong, unique password for each account. Consider a reputable password manager.",
                "Avoid personal info in passwords. Use long passphrases and enable multi-factor authentication where possible."
            }},

            { new[] { "phish", "phishing", "scam", "scams" }, new[] {
                "Be cautious of unexpected emails asking for information. Check sender addresses and never give passwords.",
                "Phishing often uses urgency. Pause, verify by contacting the organisation directly, and do not click links from unknown senders."
            }},

            { new[] { "browse", "browsing", "safe browsing", "website", "link", "links", "suspicious link" }, new[] {
                "Check URLs carefully, prefer HTTPS, and avoid downloading from untrusted sites.",
                "When in doubt, navigate to the official site manually rather than following email links."
            }},

            { new[] { "malware", "virus", "trojan" }, new[] {
                "Keep software updated, use reputable security tools, and avoid running unknown executables.",
                "Scan suspicious files and backups regularly; do not disable security warnings."
            }},

            { new[] { "social", "engineering", "social engineering" }, new[] {
                "Social engineering manipulates trust—always verify identities before sharing confidential info.",
                "Be suspicious of anyone pressuring you to take immediate action or bypass standard procedures."
            }},

            { new[] { "account", "security", "recover" }, new[] {
                "Enable MFA, keep recovery info up-to-date, and use unique passwords per account.",
                "Review account activity regularly and remove unrecognised devices or sessions."
            }}
        };

        public static string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "I didn't get that. Could you rephrase your question?";
            }

            input = input.ToLowerInvariant();

            // Direct simple conversational replies
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
                return "You can ask me about password safety, phishing, safe browsing, malware, suspicious links, account security, or social engineering.";
            }

            // Keyword/topic matching
            foreach (var kvp in TopicResponses)
            {
                if (kvp.Key.Any(k => input.Contains(k)))
                {
                    var responses = kvp.Value;
                    return responses[_rng.Next(responses.Length)];
                }
            }

            // Recognise a few simple sentiments to be friendly (Part 1 basic)
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("anxious"))
            {
                return "It's understandable to feel worried. I can share some practical steps to reduce your risk—what would you like to know first?";
            }

            if (input.Contains("thank") || input.Contains("thanks"))
            {
                return "You're welcome! Glad I could help.";
            }

            // Fallback with guidance
            return "I didn't quite understand that. Could you rephrase? You can ask about passwords, phishing, safe browsing, malware, suspicious links, account security or social engineering.";
        }
    }
}