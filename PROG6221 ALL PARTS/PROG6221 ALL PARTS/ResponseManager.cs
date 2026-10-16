using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityAwarenessBot
{
    public static class ResponseManager
    {
        private static readonly Random _rng = new();

        // Short general question -> concise answer map
        private static readonly Dictionary<string, string> GeneralAnswers = new()
        {
            { "what is phishing", "Phishing is a type of scam where attackers impersonate trustworthy organisations to steal your data. Look for spelling mistakes, unexpected urgency and mismatched sender addresses." },
            { "what is malware", "Malware is malicious software (viruses, trojans, ransomware) that can damage or take control of your device. Keep software updated and avoid unknown downloads." },
            { "what is 2fa", "Two-factor authentication (2FA) adds a second step to verify your identity, like a code from an authenticator app or SMS. It makes accounts much harder to compromise." },
            { "how to create a strong password", "Use a long passphrase or complex password unique to each account. Consider a password manager and enable multi-factor authentication where available." },
            { "how to report phishing", "Report phishing to the organisation being impersonated and to your email provider. In South Africa you can also report to the relevant authorities or contact the company directly via an official channel." }
        };

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
            return GetResponse(input, null);
        }

        public static string GetResponse(string input, ChatMemory? memory)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "I didn't get that. Could you rephrase your question?";
            }

            input = input.ToLowerInvariant();

            // If the user asks for 'another' tip or follow-up, try to continue last topic
            if (memory != null && (input.Contains("another") || input.Contains("another tip") || input.Contains("tell me more") || input.Contains("explain more") || input.Equals("more") || input.Contains("more info") || input.Contains("more details")))
            {
                if (!string.IsNullOrEmpty(memory.LastTopic))
                {
                    // find topic by token
                    foreach (var kvp in TopicResponses)
                    {
                        if (kvp.Key.Any(k => k == memory.LastTopic || memory.LastTopic.Contains(k)))
                        {
                            return PickResponseAvoidRepeat(kvp.Value, kvp.Key[0], memory);
                        }
                    }
                }
            }

            // Direct simple conversational replies
            if (input.Contains("how are you"))
            {
                return "I'm doing great! I'm ready to help you learn more about staying safe online.";
            }

            // General question matching (explicit keys -> answers)
            foreach (var kv in GeneralAnswers)
            {
                if (input.Contains(kv.Key))
                {
                    return kv.Value;
                }
            }

            if (input.Contains("purpose") || input.Contains("what do you do"))
            {
                return "My purpose is to provide cybersecurity awareness information and help you understand safer online practices.";
            }

            if (input.Contains("what can i ask") || input.Contains("what can i ask you about"))
            {
                return "You can ask me about password safety, phishing, safe browsing, malware, suspicious links, account security, or social engineering.";
            }

            // Check if the user declares interest: "i'm interested in X" or "i like X"
            var interestPhrases = new[] { "i'm interested in", "i am interested in", "i like", "i'm into", "i am into", "interested in" };
            foreach (var phrase in interestPhrases)
            {
                var idx = input.IndexOf(phrase, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    var remainder = input.Substring(idx + phrase.Length).Trim();
                    if (!string.IsNullOrEmpty(remainder))
                    {
                        // try to map remainder to a known topic token
                        foreach (var kvp in TopicResponses)
                        {
                            if (kvp.Key.Any(k => remainder.Contains(k)))
                            {
                                var canonical = kvp.Key[0];
                                memory?.Data.Remove("interest");
                                if (memory != null)
                                {
                                    memory.Data["interest"] = canonical;
                                }
                                if (memory != null)
                                {
                                    memory.LastTopic = canonical;
                                    memory.Save();
                                }
                                return $"Great! I'll remember that you're interested in {canonical}. Ask me for tips on that topic anytime.";
                            }
                        }

                        // If not matched, save raw remainder as interest
                        memory?.Data.Remove("interest");
                        if (memory != null)
                        {
                            memory.Data["interest"] = remainder;
                            memory.LastTopic = remainder;
                            memory.Save();
                        }
                        return $"Great! I'll remember that you're interested in {remainder}.";
                    }
                }
            }

            // Keyword/topic matching
            foreach (var kvp in TopicResponses)
            {
                if (kvp.Key.Any(k => input.Contains(k)))
                {
                    // store last topic for follow-ups
                    var canonical = kvp.Key[0];
                    if (memory != null)
                    {
                        memory.LastTopic = canonical;
                        memory.Save();
                    }

                    return PickResponseAvoidRepeat(kvp.Value, canonical, memory);
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

        private static string PickResponseAvoidRepeat(string[] responses, string canonicalTopic, ChatMemory? memory)
        {
            if (responses == null || responses.Length == 0)
            {
                return string.Empty;
            }

            int lastIndex = -1;
            string key = $"lastIndex:{canonicalTopic}";
            if (memory != null && memory.Data.TryGetValue(key, out var lastStr) && int.TryParse(lastStr, out var parsed))
            {
                lastIndex = parsed;
            }

            int idx = _rng.Next(responses.Length);
            if (responses.Length > 1)
            {
                int attempts = 0;
                while (idx == lastIndex && attempts < 8)
                {
                    idx = _rng.Next(responses.Length);
                    attempts++;
                }
            }

            if (memory != null)
            {
                memory.Data[key] = idx.ToString();
                memory.Save();
            }

            return responses[idx];
        }
    }
}
