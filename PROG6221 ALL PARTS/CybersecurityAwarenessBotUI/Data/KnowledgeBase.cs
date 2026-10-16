using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Data
{
    /// <summary>
    /// All of the bot's content in one place: topics (keywords, tips, details)
    /// and sentiment keywords/openers. Stored in dictionaries so the engine can
    /// be expanded in Part 3 without touching the logic.
    /// </summary>
    public static class KnowledgeBase
    {
        /// <summary>Topics keyed by a short id. Insertion order is the tie-break order.</summary>
        public static readonly Dictionary<string, Topic> Topics = BuildTopics();

        /// <summary>Used when the user shares a feeling but no specific topic.</summary>
        public static readonly Topic General = new(
            "general", "staying safe online",
            new[] { "cybersecurity", "cyber security", "online safety" },
            "Good online habits protect you everywhere.",
            new[]
            {
                "Use unique passwords and turn on multi-factor authentication wherever you can.",
                "Pause before you click: if a message feels urgent or too good to be true, verify it independently.",
                "Keep your devices and apps updated, and back up your important files.",
                "Think before you share. Personal details posted online can be used against you."
            },
            new[]
            {
                "The big four habits are strong unique passwords with MFA, thinking before you click, keeping software updated, and limiting what you share.",
                "You don't need to fix everything at once. Pick one habit to improve this week, such as turning on MFA for your email, and build from there.",
                "Most attacks succeed because of rushed decisions, not clever technology. Slowing down is one of the strongest defences you have."
            },
            "keep building one small safe habit at a time.");

        /// <summary>Keywords that signal each sentiment (matched as word starts).</summary>
        public static readonly Dictionary<Sentiment, string[]> SentimentKeywords = new()
        {
            { Sentiment.Worried,     new[] { "worried", "worry", "scared", "afraid", "anxious", "nervous", "concerned", "frightened", "paranoid", "unsafe" } },
            { Sentiment.Frustrated,  new[] { "frustrat", "annoy", "angry", "fed up", "sick of", "tired of", "irritat", "hate" } },
            { Sentiment.Overwhelmed, new[] { "overwhelm", "too much", "too complicated", "so many", "stressed" } },
            { Sentiment.Confused,    new[] { "confus", "don't understand", "do not understand", "dont understand", "unsure", "not sure", "no idea", "unclear", "what do you mean" } },
            { Sentiment.Curious,     new[] { "curious", "wonder", "want to learn", "keen to" } },
            { Sentiment.Positive,    new[] { "glad", "awesome", "excited", "great", "feel safe", "feeling good", "this is helpful" } }
        };

        /// <summary>Empathetic openers per sentiment. {name} is replaced with the user's name.</summary>
        public static readonly Dictionary<Sentiment, string[]> SentimentOpeners = new()
        {
            { Sentiment.Worried, new[] {
                "It's completely understandable to feel that way, {name}. Cyber threats can be very convincing. Let me share a tip to help you feel more in control:",
                "Feeling worried is natural, {name}, and it shows you take your safety seriously. Here's something practical you can do right now:",
                "Don't worry, {name}, you're not alone and small steps make a big difference. Try this:" } },
            { Sentiment.Frustrated, new[] {
                "I hear you, {name}. Security advice can feel like a lot of effort. Let's keep it simple:",
                "That's a fair frustration, {name}. Let's cut through the noise with one clear step:" } },
            { Sentiment.Overwhelmed, new[] {
                "Take a breath, {name}. You don't have to fix everything at once. Start with just this one thing:",
                "It can feel like a lot, {name}, but you only need one small step at a time. Here's a good place to start:" } },
            { Sentiment.Confused, new[] {
                "No problem, {name}. Let me explain that a bit more clearly:",
                "That's okay, {name}, this stuff isn't obvious. Let me break it down:" } },
            { Sentiment.Curious, new[] {
                "Great question, {name}! Curiosity is a brilliant security habit. Here's what you should know:",
                "I love the curiosity, {name}! Here's something worth knowing:" } },
            { Sentiment.Positive, new[] {
                "Glad to hear it, {name}!",
                "Great to hear, {name}! Let's keep that momentum going." } }
        };

        private static Dictionary<string, Topic> BuildTopics() => new()
        {
            { "password", new Topic("password", "password safety",
                new[] { "password", "passphrase", "passcode", "credentials" },
                "Strong passwords are the foundation of staying safe online.",
                new[]
                {
                    "Use a strong, unique password for every account. If one site is breached, your other accounts stay safe.",
                    "Longer beats clever: a passphrase of four or more random words is stronger and easier to remember than 'P@ssw0rd1'.",
                    "Avoid personal details like birthdays, pet names or ID numbers in your passwords. Attackers try these first.",
                    "A reputable password manager can generate and store unique passwords so you only need to remember one master passphrase.",
                    "Turn on multi-factor authentication wherever possible so a stolen password alone isn't enough."
                },
                new[]
                {
                    "Why unique passwords matter: criminals use 'credential stuffing', trying leaked email and password pairs on other sites. Reusing a password turns one breach into many.",
                    "To build a passphrase, pick four unrelated words and add a number or symbol between them. Avoid song lyrics or famous quotes. Length is what makes it strong.",
                    "If you suspect a password is exposed, change it immediately, change it anywhere you reused it, and check your email for unknown logins or forwarding rules."
                },
                "you may want to check that your most important accounts, like email and banking, each have their own long passphrase.") },

            { "phishing", new Topic("phishing", "phishing and scams",
                new[] { "phish", "scam", "fraud", "fake email", "spam", "smishing", "vishing", "suspicious email" },
                "Recognising scams is one of the most valuable skills you can build.",
                new[]
                {
                    "Be cautious of emails or messages asking for personal information. Scammers often pose as banks, SARS or couriers.",
                    "Phishing relies on urgency. If a message says 'act now or lose access', pause and verify by contacting the organisation on its official number.",
                    "Check the sender's address and hover over links before clicking. Look for small spelling changes in the domain name.",
                    "Your bank will never ask for your PIN, password or OTP by email, SMS or phone call. If someone does, hang up and call the number on your card.",
                    "Unexpected attachments are a red flag, especially invoices, delivery notices or 'voice notes' from people you don't know."
                },
                new[]
                {
                    "Common signs of phishing: generic greetings, spelling errors, mismatched links, threats, and requests for sensitive information. Any one of these is a reason to slow down.",
                    "There are variants: smishing is phishing by SMS, vishing is by phone call, and spear phishing targets you personally using details found online.",
                    "If you clicked a suspicious link: disconnect if you downloaded something, change any passwords you entered, contact your bank if financial details were shared, and report the message."
                },
                "you can help protect others by reporting phishing messages to the organisation being impersonated.") },

            { "privacy", new Topic("privacy", "privacy",
                new[] { "privacy", "personal information", "personal data", "popia", "data protection", "tracking", "share my data" },
                "It's a crucial part of staying safe online.",
                new[]
                {
                    "Review the privacy settings on your social media accounts and limit who can see your posts, phone number and location.",
                    "Share the minimum: before giving out personal details, ask why they are needed and how they'll be protected. In South Africa, POPIA gives you rights over your personal information.",
                    "Check app permissions regularly. A torch app doesn't need your contacts or microphone.",
                    "Avoid posting details like your ID number, home address, travel plans or boarding passes online.",
                    "Consider a tracker-blocking extension or a privacy-focused browser to reduce how much advertisers follow you."
                },
                new[]
                {
                    "POPIA, the Protection of Personal Information Act, requires organisations to handle your personal information lawfully and securely, and lets you ask what they hold about you.",
                    "Small details add up. Scammers combine public facts like your employer, birthday and pet's name to guess passwords or impersonate you.",
                    "Do a quick privacy check-up: search your own name, review which apps have access to your accounts, delete old accounts you no longer use, and tighten the settings on each platform."
                },
                "you might want to review the security and privacy settings on your accounts every few months.") },

            { "browsing", new Topic("browsing", "safe browsing",
                new[] { "browse", "browsing", "website", "https", "url", "link", "download", "wifi", "wi-fi", "public network", "suspicious link" },
                "Safe browsing habits protect you every single day.",
                new[]
                {
                    "Check the URL carefully and look for HTTPS. Remember the padlock only means the connection is encrypted, not that the site is trustworthy.",
                    "When in doubt, type the official address yourself instead of following a link in an email or message.",
                    "Only download software and apps from official stores or the vendor's own website.",
                    "Avoid banking or entering passwords on public Wi-Fi. Use mobile data or a trusted VPN instead.",
                    "Keep your browser up to date and be wary of pop-ups urging you to install 'urgent' updates or plug-ins."
                },
                new[]
                {
                    "Lookalike domains swap letters (like 'rn' for 'm') or add extra words, such as 'yourbank-secure-login.com'. Read the domain name, the part before the first '/', carefully.",
                    "Public Wi-Fi can be imitated by attackers using a similar network name. If you must use it, avoid sensitive logins and make sure sites use HTTPS.",
                    "Before opening a suspicious link, preview it by long-pressing on mobile or hovering on desktop, without actually visiting it."
                },
                "you could make a habit of checking the address bar before you log in anywhere.") },

            { "malware", new Topic("malware", "malware",
                new[] { "malware", "virus", "trojan", "ransomware", "spyware", "antivirus", "anti-virus", "infected" },
                "Understanding malware helps you avoid the most damaging attacks.",
                new[]
                {
                    "Keep your operating system, browser and apps updated. Many infections exploit known flaws that have already been fixed.",
                    "Use reputable security software and don't disable its warnings.",
                    "Never open attachments or run programs from people or sites you don't trust.",
                    "Back up important files regularly, ideally with one copy offline, so ransomware can't hold them hostage.",
                    "Be wary of pirated software and cracked games. They're a popular way to spread malware."
                },
                new[]
                {
                    "Malware is an umbrella term: viruses spread by infecting files, trojans pretend to be useful software, spyware watches what you do, and ransomware locks your files until you pay.",
                    "Signs of infection include a sudden slowdown, unknown programs, constant pop-ups, or a browser home page that changed. If you notice them, disconnect from the internet and run a full scan.",
                    "Use the 3-2-1 backup rule: keep three copies of your data, on two different types of storage, with one stored off-site or offline."
                },
                "keeping regular backups and updates is the best habit you can build.") },

            { "social", new Topic("social", "social engineering",
                new[] { "social engineering", "impersonat", "pretext", "manipulat", "pretending to be", "baiting", "tailgating", "someone called me" },
                "People are often the weakest link, so awareness really pays off.",
                new[]
                {
                    "Social engineering manipulates trust rather than technology. Always verify someone's identity before sharing confidential information.",
                    "Be suspicious of anyone pressuring you to act immediately or to bypass normal procedures.",
                    "Attackers research you first. Be careful how much personal and work information you share on social media.",
                    "If a caller claims to be from your bank, IT department or a delivery company, end the call and phone the official number yourself.",
                    "Never hand over a one-time PIN, even to someone who sounds official or already knows some of your details."
                },
                new[]
                {
                    "Typical tricks include pretexting (a made-up story), baiting (a 'free' USB or download), tailgating (following you into a secure area) and impersonating authority figures.",
                    "Scammers exploit emotions like fear, urgency, curiosity and the wish to be helpful. Naming the emotion you're feeling helps you slow down.",
                    "A good rule is to verify through a second channel. If your 'boss' emails an urgent payment request, call them on a number you already know before paying."
                },
                "a good habit is to verify any unexpected request through a second channel.") },

            { "account", new Topic("account", "account security",
                new[] { "account", "two-factor", "two factor", "2fa", "mfa", "multi-factor", "authenticat", "otp", "sim swap", "recover", "hacked", "security settings", "login", "log in" },
                "Locking down your accounts blocks a huge share of attacks.",
                new[]
                {
                    "Enable multi-factor authentication on your email, banking and social media accounts.",
                    "Keep your recovery email and phone number up to date so you can regain access if you're locked out.",
                    "Review your account activity regularly and remove devices or sessions you don't recognise.",
                    "Prefer an authenticator app over SMS codes where possible, as it protects against SIM-swap fraud.",
                    "Protect your email account most of all. Whoever controls it can reset the rest of your passwords."
                },
                new[]
                {
                    "MFA combines something you know (your password) with something you have (your phone or a security key). Even if criminals get your password, they still can't log in.",
                    "In a SIM-swap attack, criminals trick your mobile provider into moving your number to their SIM and then intercept your codes. If your phone suddenly loses signal for no reason, contact your provider straight away.",
                    "If you think an account was hacked: change the password from a clean device, sign out all sessions, enable MFA, check recovery details and email forwarding rules, and warn your contacts."
                },
                "you might want to review the security settings on your accounts and make sure two-factor authentication is on.") },

            { "updates", new Topic("updates", "software updates",
                new[] { "update", "patch", "software update", "outdated", "operating system", "upgrade" },
                "Staying up to date closes doors that attackers rely on.",
                new[]
                {
                    "Install software and operating system updates promptly. They often fix security holes attackers already know about.",
                    "Turn on automatic updates for your phone, computer and apps so you never fall behind.",
                    "Retire software that no longer receives updates, such as old operating system versions.",
                    "Update your router's firmware too, and change its default admin password."
                },
                new[]
                {
                    "Attackers study patches to find the flaw that was fixed, then target people who haven't updated. The longer you wait, the bigger your risk.",
                    "Only install updates from official sources, such as your device's settings or the vendor's site. Fake update pop-ups are a common malware trick.",
                    "Don't forget less obvious devices. Smart TVs, cameras, routers and printers can all be entry points if left outdated."
                },
                "turning on automatic updates is a simple, high-value habit.") }
        };
    }
}
