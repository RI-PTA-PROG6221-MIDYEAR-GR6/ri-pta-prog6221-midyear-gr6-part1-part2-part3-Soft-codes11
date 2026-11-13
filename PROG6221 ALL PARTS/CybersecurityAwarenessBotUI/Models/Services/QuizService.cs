using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class QuizService
    {
        // The Part 3 quiz contains twelve cybersecurity questions with answers
        // and explanations, including multiple-choice and true/false questions.
        public IReadOnlyList<QuizQuestion> Questions { get; } = new List<QuizQuestion>
        {
            new() { Question = "What should you do with an unexpected email asking for your password?", Options = new[] { "Reply with it", "Report it as phishing", "Forward it to friends", "Click its link" }, CorrectIndex = 1, Explanation = "Legitimate organisations do not ask for your password by email. Report the message as phishing." },
            new() { Question = "Which password is strongest?", Options = new[] { "Password123", "Your birthday", "A long unique passphrase", "The same password everywhere" }, CorrectIndex = 2, Explanation = "Long, unique passwords or passphrases are harder to guess and should not be reused." },
            new() { Question = "True or false: Multi-factor authentication adds protection beyond a password.", Options = new[] { "True", "False" }, CorrectIndex = 0, Explanation = "MFA requires another proof of identity, so a stolen password alone is less useful." , IsTrueFalse = true },
            new() { Question = "What is a safer way to open a banking website?", Options = new[] { "Use a random pop-up link", "Type the official address yourself", "Use any shortened URL", "Disable browser security" }, CorrectIndex = 1, Explanation = "Navigate to the official website yourself instead of trusting unexpected links." },
            new() { Question = "What does malware mean?", Options = new[] { "Malicious software", "A strong password", "A secure backup", "A firewall rule" }, CorrectIndex = 0, Explanation = "Malware is software designed to damage, spy on, or gain unauthorised access to devices." },
            new() { Question = "True or false: Software updates can fix security weaknesses.", Options = new[] { "True", "False" }, CorrectIndex = 0, Explanation = "Updates often include security patches, so delaying them increases risk." , IsTrueFalse = true },
            new() { Question = "Which is a sign of social engineering?", Options = new[] { "A normal system update", "Pressure to act urgently and keep it secret", "A known contact greeting you", "A scheduled backup" }, CorrectIndex = 1, Explanation = "Attackers often create urgency, fear, or secrecy to bypass your normal checks." },
            new() { Question = "What should you do on public Wi-Fi?", Options = new[] { "Share passwords publicly", "Use sensitive accounts without protection", "Avoid sensitive activity or use trusted protection", "Turn off all updates" }, CorrectIndex = 2, Explanation = "Treat public networks as untrusted and avoid sensitive actions unless protected." },
            new() { Question = "True or false: You should verify a caller's identity before sharing confidential information.", Options = new[] { "True", "False" }, CorrectIndex = 0, Explanation = "Verify callers through an official channel before sharing confidential data." , IsTrueFalse = true },
            new() { Question = "What is the best response to a suspicious attachment?", Options = new[] { "Open it immediately", "Scan or verify it before opening", "Disable antivirus", "Rename it and run it" }, CorrectIndex = 1, Explanation = "Verify unexpected attachments and scan them before opening; when unsure, do not open them." },
            new() { Question = "Why should accounts use different passwords?", Options = new[] { "It makes typing faster", "It limits damage if one password is exposed", "It disables MFA", "It prevents updates" }, CorrectIndex = 1, Explanation = "Unique passwords stop one breached account from unlocking your other accounts." },
            new() { Question = "True or false: Backups can help recover from ransomware.", Options = new[] { "True", "False" }, CorrectIndex = 0, Explanation = "Current, tested backups can help restore data after destructive malware." , IsTrueFalse = true }
        };
    }
}
