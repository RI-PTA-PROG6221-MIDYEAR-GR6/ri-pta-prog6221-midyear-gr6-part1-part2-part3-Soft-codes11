# Cybersecurity Awareness Bot (PROG6221 – POE Part 2)

A WPF desktop chatbot that teaches South African users how to stay safe online.
Part 2 converts the Part 1 console app into a GUI and adds keyword recognition,
random responses, conversation flow, memory, and sentiment detection.

[![.NET Build (CI)](https://github.com/<your-username>/<your-repo>/actions/workflows/dotnet.yml/badge.svg)](https://github.com/<your-username>/<your-repo>/actions)

**Video presentation:** <paste unlisted YouTube link here>

## How to run

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download) (Windows 10/11).
2. Make sure your voice greeting is at `Assets/greetings.wav`.
3. Clone the repo, then in the project folder run:
   ```
   dotnet run
   ```
   or open `CybersecurityAwarenessBot.csproj` in Visual Studio 2022 and press **F5**.

## Features (mapped to the Part 2 brief)

| Requirement | Where / how |
|---|---|
| GUI with all Part 1 features | `MainWindow.xaml` – voice greeting on launch, ASCII logo on the welcome screen and a compact shield in the sidebar, name entry, coloured chat bubbles, typing effect |
| Keyword recognition | `Data/KnowledgeBase.cs` + `Models/Topic.cs` – 8 topics (passwords, phishing/scams, privacy, safe browsing, malware, social engineering, account security, software updates); regex word-start matching with scoring |
| Random responses | Every topic has 4–5 tips dealt from a shuffled deck (`ChatEngine.NextFromDeck`), so tips don't repeat until all have been used |
| Conversation flow | Follow-ups like *"give me another tip"*, *"tell me more"*, *"explain more"*, *"I don't understand"* continue the current topic |
| Memory and recall | `Models/User.cs` stores name, favourite topic, last mood and topics explored. Shown in the sidebar and used to personalise replies. Try *"I'm interested in privacy"* then *"what do you remember about me?"* |
| Sentiment detection | `Services/SentimentAnalyzer.cs` – worried, frustrated, overwhelmed, confused, curious, positive. The bot opens with empathy **and immediately shares a tip** |
| Error handling | Empty input, unknown input, invalid names, audio failures and unexpected exceptions are all handled without crashing |
| Code optimisation | Dictionaries (topics, sentiments), lists, `Queue<int>` decks, **delegates** (`IntentHandler` pipeline, `Action<User>` event), separate classes by responsibility |

## Project structure

```
CybersecurityAwarenessBot/
├── App.xaml / App.xaml.cs        Application entry point + global error handler + styles
├── MainWindow.xaml / .cs         GUI only (no chatbot logic)
├── Models/                       User (memory), Topic, Sentiment, ChatReply, ChatMessage
├── Services/
│   ├── ChatEngine.cs             Delegate pipeline: memory → recall → small talk → topics/sentiment
│   ├── SentimentAnalyzer.cs      Mood detection + empathetic openers
│   └── VoiceGreeting.cs          Plays Assets/greetings.wav on a background thread
├── Data/KnowledgeBase.cs         All topics, tips, details, sentiment keywords
├── Helpers/AsciiArt.cs           ASCII logo from Part 1
├── Assets/greetings.wav          Voice greeting
└── .github/workflows/dotnet.yml  CI: restore + build on every push
```

## Example conversation

```
You:     Hi
CyberBot: Hello, Sam! Ready to learn something new about staying safe online?
You:     I'm interested in privacy
CyberBot: Great! I'll remember that you're interested in privacy. ...
You:     I'm worried about online scams
CyberBot: It's completely understandable to feel that way, Sam. ... [phishing tip]
You:     tell me more
CyberBot: [deeper explanation on phishing]
You:     give me another tip
CyberBot: [a different phishing tip]
```

## Continuous Integration

Screenshot of a successful run: *(add your green-tick screenshot here, e.g. `docs/ci-success.png`)*

## Releases

- **v1.0.0** – Part 1 console chatbot
- **v1.1.0** – WPF GUI conversion
- **v2.0.0** – Part 2 complete (keywords, random replies, memory, sentiment)

## References

- Microsoft. 2026. *Windows Presentation Foundation documentation*. [Online]. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/ [Accessed 11 October 2026].
- Pieterse, H. 2021. The Cyber Threat Landscape in South Africa: A 10-Year Review. *The African Journal of Information and Communication*, 28(28). doi: https://doi.org/10.23962/10539/32213.
- Republic of South Africa. 2013. *Protection of Personal Information Act 4 of 2013*. Pretoria: Government Printer.
