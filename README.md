# Cybersecurity Awareness Bot

**PROG6221 – Programming 2A Portfolio of Evidence (Parts 1, 2 and 3)**

## Project Overview

The Cybersecurity Awareness Bot is a C# application developed to promote cybersecurity awareness and help users understand common online security threats. The project was developed in three parts, progressing from a console-based chatbot to a graphical user interface with additional interactive features.

The repository contains:

- **Part 1:** A .NET 9 Windows console chatbot.
- **Parts 2 and 3:** A .NET 8 Windows Presentation Foundation (WPF) graphical chatbot.

## Requirements

- Windows 10 or later.
- .NET 9 SDK.
- .NET 8 SDK.
- Visual Studio with the .NET desktop development and WPF workload.
<<<<<<< HEAD
- MySQL Server 8.x (optional; only required when database-backed task storage is desired).

The WPF application does not require MySQL to start. If the MySQL connection is not
configured or cannot be opened, task data and activity data are stored as JSON files
under the application's `Data` directory.

## Running the Applications

Open PowerShell in the repository's root directory.

### Restore and build the solution

```powershell
dotnet restore "PROG6221 ALL PARTS/CybersecurityAwarenessBot.sln"

dotnet build "PROG6221 ALL PARTS/CybersecurityAwarenessBot.sln" --configuration Release
```

### Run Part 1 – Console Application

```powershell
dotnet run --project "PROG6221 ALL PARTS/PROG6221 ALL PARTS/CybersecurityAwarenessBot.csproj"
```

The console application plays `Assets/greetings.wav` when the file is available and
continues with a text-only greeting if audio playback is unavailable.

### Run Parts 2 and 3 – WPF Application

```powershell
dotnet run --project "PROG6221 ALL PARTS/CybersecurityAwarenessBotUI/CybersecurityAwarenessBotUI.csproj"
```

The WPF application starts with a name-entry screen. Enter a name and select
**Start chatting** (or press Enter) before using the chat, sidebar, task assistant,
quiz, or activity-log features. The sidebar can be opened or closed with the
hamburger button in the chat header.

The WPF application can also be opened through the solution file:

`PROG6221 ALL PARTS/CybersecurityAwarenessBot.sln`

When using Visual Studio, right-click `CybersecurityAwarenessBotUI` in Solution
Explorer and select **Set as Startup Project** before pressing **F5**. The solution
also contains the separate Part 1 console project, so Visual Studio may otherwise
launch the console project when no local startup-project preference exists. The
command-line `dotnet run --project` command above always launches the WPF project
explicitly.

## Part 1 – Console Chatbot Features

- Cybersecurity-themed ASCII logo.
- Personalised greeting using the user's name.
- Recorded WAV voice greeting with Windows audio playback and fallback handling.
- Responses covering passwords, phishing, privacy and safe browsing.
- Input validation for empty and unrecognised messages.
- Coloured console interface, borders and typing effect.
- Separate classes for chatbot logic, console interface, responses, memory and voice greeting.

The console voice greeting is stored in:

`PROG6221 ALL PARTS/PROG6221 ALL PARTS/Assets/greetings.wav`

## Part 2 – Graphical User Interface Features

- WPF graphical user interface.
- Personalised interaction and voice greeting.
- Cybersecurity keyword recognition.
- Randomised topic responses.
- Follow-up conversation flow.
- Conversation memory and recall.
- Sentiment detection for worried, frustrated, confused, overwhelmed, curious and positive messages.
- Use of delegates, dictionaries, lists and queues.
- Handling of invalid and unrecognised input.

## Part 3 – Advanced Features

### Task Assistant and Reminders

- Add tasks with a title and description.
- Set optional reminder dates.
- View saved tasks.
- Mark tasks as complete.
- Delete tasks.

### Database Integration

- MySQL task storage when `CYBERBOT_MYSQL_CONNECTION_STRING` is configured.
- Automatic creation of the `cyber_tasks` table when the configured database is reachable.
- Task creation, retrieval, updating and deletion.
- JSON storage fallback when MySQL is not configured or unavailable.

The application does not create the MySQL server, database, or user. Those must exist
before using database storage. The connection string must identify an existing
database, for example:

`Server=localhost;Port=3306;Database=cybersecurity_bot;User ID=cyberbot;Password=YOUR_PASSWORD;`

### Cybersecurity Quiz

- Twelve cybersecurity questions.
- Multiple-choice and true/false questions.
- One question displayed at a time.
- Immediate feedback after answering.
- Explanations for answers.
- Score tracking and final feedback.

=======
- MySQL Server 8.x (optional, for Part 3 database functionality).

## Running the Applications

Open PowerShell in the repository's root directory.

### Restore and build the solution

```powershell
dotnet restore "PROG6221 ALL PARTS/CybersecurityAwarenessBot.sln"

dotnet build "PROG6221 ALL PARTS/CybersecurityAwarenessBot.sln" --configuration Release
```

### Run Part 1 – Console Application

```powershell
dotnet run --project "PROG6221 ALL PARTS/PROG6221 ALL PARTS/CybersecurityAwarenessBot.csproj"
```

### Run Parts 2 and 3 – WPF Application

```powershell
dotnet run --project "PROG6221 ALL PARTS/CybersecurityAwarenessBotUI/CybersecurityAwarenessBotUI.csproj"
```

The WPF application can also be opened through the solution file:

`PROG6221 ALL PARTS/CybersecurityAwarenessBot.sln`

## Part 1 – Console Chatbot Features

- Cybersecurity-themed ASCII logo.
- Personalised greeting using the user's name.
- Recorded WAV voice greeting with Windows audio playback and fallback handling.
- Responses covering passwords, phishing, privacy and safe browsing.
- Input validation for empty and unrecognised messages.
- Coloured console interface, borders and typing effect.
- Separate classes for chatbot logic, console interface, responses, memory and voice greeting.

The console voice greeting is stored in:

`PROG6221 ALL PARTS/PROG6221 ALL PARTS/Assets/greetings.wav`

## Part 2 – Graphical User Interface Features

- WPF graphical user interface.
- Personalised interaction and voice greeting.
- Cybersecurity keyword recognition.
- Randomised topic responses.
- Follow-up conversation flow.
- Conversation memory and recall.
- Sentiment detection for worried, frustrated, confused, overwhelmed, curious and positive messages.
- Use of delegates, dictionaries, lists and queues.
- Handling of invalid and unrecognised input.

## Part 3 – Advanced Features

### Task Assistant and Reminders

- Add tasks with a title and description.
- Set optional reminder dates.
- View saved tasks.
- Mark tasks as complete.
- Delete tasks.

### Database Integration

- MySQL task storage.
- Automatic creation of the task table when configured.
- Task creation, retrieval, updating and deletion.
- JSON storage fallback when MySQL is not configured or unavailable.

### Cybersecurity Quiz

- Twelve cybersecurity questions.
- Multiple-choice and true/false questions.
- One question displayed at a time.
- Immediate feedback after answering.
- Explanations for answers.
- Score tracking and final feedback.

>>>>>>> origin/main
### Natural Language Processing Simulation

- Keyword detection and string manipulation.
- Recognition of different ways of expressing supported commands.
- Task and reminder commands.
- Quiz-related commands.
- Activity-log requests.

### Activity Log

- Records supported user actions.
- Includes task-related activities, reminders, quiz activities and chatbot commands.
- Displays recent activity entries.
- Supports requests such as "Show activity log" and "What have you done for me?"

## MySQL Database Configuration

The application uses a Windows environment variable for the MySQL connection string. Do not commit real database passwords or credentials to GitHub.

<<<<<<< HEAD
Set the connection string in the same PowerShell session used to launch the WPF application:
=======
Set the connection string in PowerShell before running the WPF application:
>>>>>>> origin/main

```powershell
$env:CYBERBOT_MYSQL_CONNECTION_STRING = "Server=localhost;Port=3306;Database=cybersecurity_bot;User ID=cyberbot;Password=YOUR_PASSWORD;"

dotnet run --project "PROG6221 ALL PARTS/CybersecurityAwarenessBotUI/CybersecurityAwarenessBotUI.csproj"
```

Replace `YOUR_PASSWORD` with your own local MySQL password. Keep the real connection string private.

<<<<<<< HEAD
When MySQL is not configured or cannot be reached, the application uses JSON fallback storage:

`Data/cyber-tasks.json` for tasks and reminders, and `Data/activity-log.json` for activity history.
=======
When MySQL is not configured or cannot be reached, the application uses its JSON fallback storage:

`Data/cyber-tasks.json`
>>>>>>> origin/main

## Project Structure

```text
PROG6221 ALL PARTS/
│
├── CybersecurityAwarenessBot.sln
│
├── PROG6221 ALL PARTS/
│   ├── Assets/
│   │   └── greetings.wav
<<<<<<< HEAD
    │   ├── Program.cs
    │   ├── ChatBot.cs
=======
│   ├── Program.cs
│   ├── Chatbot.cs
>>>>>>> origin/main
│   ├── ConsoleUI.cs
│   └── VoiceGreeting.cs
│
└── CybersecurityAwarenessBotUI/
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
<<<<<<< HEAD
    ├── Helpers/
    ├── Models/
    │   └── Services/
=======
    ├── Models/
    ├── Services/
>>>>>>> origin/main
    ├── Data/
    └── Assets/
        └── greetings.wav

.github/
└── workflows/
```

## Continuous Integration

GitHub Actions workflow files are located in `.github/workflows/`.

The workflows are configured to restore and build the solution on Windows when the relevant push and pull-request events occur.

## Video Presentation

The video presentation demonstrates the application, its features, code structure, logic and implementation.

- **Part 1 presentation:** [Insert unlisted video link]
- **Part 2 presentation:** [Insert unlisted video link]
- **Final Part 3 POE presentation:** [Insert unlisted video link]

## Releases and Version History

The project is developed in stages:

- Part 1 – Console chatbot.
- Part 2 – WPF graphical chatbot.
- Part 3 – Final enhanced chatbot with task management, database integration, quiz functionality, natural-language-style commands and activity logging.

GitHub commits and releases provide the project's development history.

## Academic References

Microsoft. (2026). *Windows Presentation Foundation documentation*. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/ (Accessed: 2026).

Pieterse, H. (2021). The Cyber Threat Landscape in South Africa: A 10-Year Review. *The African Journal of Information and Communication*, 28(28). https://doi.org/10.23962/10539/32213.

<<<<<<< HEAD
Republic of South Africa. (2013). *Protection of Personal Information Act 4 of 2013*. Pretoria: Government Printer.ALTER TABLE cyber_tasks ADD COLUMN IF NOT EXISTS owner_name ...
=======
Republic of South Africa. (2013). *Protection of Personal Information Act 4 of 2013*. Pretoria: Government Printer.
>>>>>>> origin/main
